using DBLibrary;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using TelegramLibrary;
using TelegramTimer.Infrastructure;
using TelegramTimer.Model;
using TeleSharp.TL;

namespace TelegramTimer.ViewModel
{
    public class MainWindowViewModel : BaseViewModel
    {
        private List<string> FileSessionStore;


        public TelegramLogic TelegramContext { get; set; }



        private List<string> _channelsName;
        public List<string> ChannelsNames
        {
            get { return _channelsName; }
            set { _channelsName = value; OnPropertyChanged(); }
        }



        private List<TLChannelFull> _tLChannelFulls;
        public List<TLChannelFull> TLChannelFulls
        {
            get { return _tLChannelFulls; }
            set { _tLChannelFulls = value; OnPropertyChanged(); }
        }

        private List<TLChannel> _tLChannels;

        public List<TLChannel> TLChannels
        {
            get { return _tLChannels; }
            set { _tLChannels = value; OnPropertyChanged(); }
        }




        private string _phoneNumber;
        public string PhoneNumber
        {
            get { return _phoneNumber; }
            set { _phoneNumber = value; OnPropertyChanged(); }
        }

        private string _authCode;
        public string AuthCode
        {
            get { return _authCode; }
            set { _authCode = value; OnPropertyChanged(); }
        }

        private string _photoPath;
        public string PhotoPath
        {
            get { return _photoPath; }
            set { _photoPath = value; OnPropertyChanged(); }
        }

        private string _hours;
        public string Hours
        {
            get { return _hours; }
            set
            {
                int tempNum;
                if (Int32.TryParse(value, out tempNum))
                {
                    if (tempNum > 23)
                        value = "0";
                    else if (tempNum < 0)
                        value = "0";

                    if (tempNum < DateTime.Now.Hour)
                        value = $"{DateTime.Now.Hour + 1}";

                    _hours = value;
                    OnPropertyChanged();
                }
                else
                    TeleStatus = "Введите час";
            }
        }

        private string _minutes;
        public string Minutes
        {
            get { return _minutes; }
            set
            {
                int tempNum;
                if (Int32.TryParse(value, out tempNum))
                {
                    if (tempNum > 59)
                        value = "0";
                    else if (tempNum < 0)
                        value = "0";

                    int hour;
                    if (Int32.TryParse(Hours, out hour))
                    {
                        if (hour <= DateTime.Now.Hour && tempNum <= DateTime.Now.Minute)
                        {
                            TeleStatus = "Указано прошлое\nвремя.";
                            return;
                        }
                        _minutes = value;
                        OnPropertyChanged();
                    }
                    else
                        TeleStatus = "Сначала укажите час";
                }
                else
                    TeleStatus = "Введите минуты.";
            }
        }

        private string _textSending;

        public string TextSending
        {
            get { return _textSending; }
            set { _textSending = value; OnPropertyChanged(); }
        }


        private string _teleStatus;
        public string TeleStatus
        {
            get { return _teleStatus; }
            set { _teleStatus = value; OnPropertyChanged(); }
        }

        private string _mySelectedItem;

        public string MySelectedItem
        {
            get { return _mySelectedItem; }
            set { _mySelectedItem = value; OnPropertyChanged(); }
        }





        private int _successfullyRequested;
        public int SuccessfullyRequested
        {
            get { return _successfullyRequested; }
            set { _successfullyRequested = value; OnPropertyChanged(); }
        }

        private int _badRequested;
        public int BadRequested
        {
            get { return _badRequested; }
            set { _badRequested = value; OnPropertyChanged(); }
        }

        private int _waitingRequest;
        public int WaitingRequest
        {
            get { return _waitingRequest; }
            set { _waitingRequest = value; OnPropertyChanged(); }
        }




        private DateTime _startDateTime;
        public DateTime StartDateTime
        {
            get { return _startDateTime; }
            set
            {
                if (value < DateTime.Now)
                    value = DateTime.Now;

                _startDateTime = value;
                OnPropertyChanged();
            }
        }


        private bool _isSetCode;
        public bool IsSetCode
        {
            get { return _isSetCode; }
            set { _isSetCode = value; OnPropertyChanged(); }
        }

        private bool _isLoggined;
        public bool IsLoggEnabled
        {
            get { return _isLoggined; }
            set { _isLoggined = value; OnPropertyChanged(); }
        }

        private bool _isProgramStarted;
        public bool IsProgramStarted
        {
            get { return _isProgramStarted; }
            set { _isProgramStarted = value; OnPropertyChanged(); }
        }

        private bool _isStartEnabled;
        public bool IsStartEnabled
        {
            get { return _isStartEnabled; }
            set { _isStartEnabled = value; OnPropertyChanged(); }
        }

        private bool _isStopEnabled;

        public bool IsStopEnabled
        {
            get { return _isStopEnabled; }
            set { _isStopEnabled = value; OnPropertyChanged(); }
        }
        private bool _isCanChangeNumber;
        public bool IsCanChangeNumber
        {
            get { return _isCanChangeNumber; }
            set { _isCanChangeNumber = value; OnPropertyChanged(); }
        }



        public ICommand Login { get; set; }
        public ICommand SendCode { get; set; }
        public ICommand StartCommand { get; set; }
        public ICommand StopCommand { get; set; }
        public ICommand ChoosePhotoCommand { get; set; }
        public ICommand AddQueueCommand { get; set; }
        public ICommand GetSummaryCommand { get; set; }
        public ICommand ClearPhotoPathCommand { get; set; }


        private ObservableCollection<DataGridQueryModel> _dataGridQueryModels;

        public ObservableCollection<DataGridQueryModel> DataGridQueries
        {
            get { return _dataGridQueryModels; }
            set { _dataGridQueryModels = value; OnPropertyChanged(); }
        }


        public MainWindowViewModel()
        {
            Login = new RelayCommand(OnLoginCommandExecute);
            SendCode = new RelayCommand(OnSendCodeCommandExecute);
            StartCommand = new RelayCommand(OnStartCommandExecute);
            ChoosePhotoCommand = new RelayCommand(OnChoosePhotoCommandExecute);
            AddQueueCommand = new RelayCommand(OnAddQueueCommandExecute);
            StopCommand = new RelayCommand(OnStopCommandExecute);
            GetSummaryCommand = new RelayCommand(OnGetSummaryCommandExecute);
            ClearPhotoPathCommand = new RelayCommand(OnClearPhotoPathCommandExecute);

            DataGridQueries = new ObservableCollection<DataGridQueryModel>();

            List<string> tmpfiles = new List<string>();
            try
            {
                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Sessions");
                tmpfiles = Directory.GetFiles(path, "*", SearchOption.AllDirectories).ToList();
            }
            catch { }

            if (tmpfiles.Count > 0)
                PhoneNumber = Path.GetFileName(tmpfiles[0]).TrimEnd('.', 'd', 'a', 't');
            else
                PhoneNumber = "";
            TeleStatus = "Введите номер телефона";

            WaitingRequest = 0;
            BadRequested = 0;
            SuccessfullyRequested = 0;

            FileSessionStore = new List<string>();
            ChannelsNames = new List<string>();

            StartDateTime = DateTime.Now;

            IsSetCode = false;
            IsCanChangeNumber = true;
            IsLoggEnabled = true;
            IsStartEnabled = false;
            IsStopEnabled = false;
        }


        public async void OnLoginCommandExecute(object obj)
        {
            await Task.Run(async () =>
            {
                Regex regex = new Regex(@"^[+]*[(]{0,1}[0-9]{1,4}[)]{0,1}[-\s\./0-9]*$");
                if (regex.IsMatch(PhoneNumber))
                {
                    IsLoggEnabled = false;
                    TeleStatus = "Загрузка...";

                    TelegramContext = new TelegramLogic(PhoneNumber);
                    string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Sessions");
                    var tmpfiles = Directory.GetFiles(path, "*", SearchOption.AllDirectories).ToList();

                    foreach (var item in tmpfiles)
                    {
                        FileSessionStore.Add(Path.GetFileName(item));
                    }
                    foreach (var item in FileSessionStore)
                    {
                        if (item.Equals(PhoneNumber + ".dat"))
                        {
                            await TelegramContext.LogInAsync();


                            TLChannelFulls = await TelegramContext.GetFullInfoCannelsAsync();
                            foreach (var channel in TLChannelFulls)
                            {
                                ChannelsNames.Add(channel.About);
                            }

                            MySelectedItem = ChannelsNames?[0];
                            IsProgramStarted = true;
                            IsStartEnabled = true;
                            IsCanChangeNumber = false;
                            TeleStatus = "Вы вошли успешно!!! ";
                            TelegramContext.LogOut();
                            return;
                        }
                    }

                    try
                    {
                        await TelegramContext.GetCodeAuthenticateAsync();
                        TeleStatus = "Ждите код для подтверждения";
                        IsSetCode = true;
                    }
                    catch (Exception ex)
                    {
                        TeleStatus = ex.Message;
                        File.Delete(PhoneNumber + ".dat");
                        IsLoggEnabled = true;
                    }
                }
                else
                    TeleStatus = "Некорректный номер!\nПопробуйте: +1234...";
            });
        }

        public async void OnSendCodeCommandExecute(object obj)
        {
            await Task.Run(async () =>
            {
                TeleStatus = "Ожидание подтверждения";

                IsSetCode = false;
                TelegramContext.Code = AuthCode;
                try
                {
                    await TelegramContext.AuthUserAsync();


                    TeleStatus = "Вы вошли успешно!!!";
                    IsProgramStarted = true;
                    IsStartEnabled = true;
                    IsCanChangeNumber = false;

                    TLChannelFulls = await TelegramContext.GetFullInfoCannelsAsync();
                    foreach (var channel in TLChannelFulls)
                    {
                        ChannelsNames.Add(channel.About);
                    }
                    TelegramContext.LogOut();
                    MySelectedItem = ChannelsNames?[0];
                }
                catch (Exception ex)
                {
                    TeleStatus = ex.Message;

                    if (File.Exists(PhotoPath + "dat"))
                        File.Delete(PhotoPath + "dat");
                }
            });
        }

        public async void OnStartCommandExecute(object obj)
        {
            IsStartEnabled = false;
            IsStopEnabled = true;

            TeleStatus = "Рассылка запущена!";
            await Task.Run(async () =>
            {
                while (IsStopEnabled)
                {
                    foreach (var item in DataGridQueries)
                    {
                        if (item.Status == "Ожидание...")
                            if (item.DateTimeSending <= DateTime.Now)
                            {
                                TelegramContext = new TelegramLogic(PhoneNumber);
                                await TelegramContext.LogInAsync();
                                bool result = false;
                                string exception = null;
                                try
                                {
                                    result = await TelegramContext.SendMessage(new TelegramLibrary.Models.SendingQueryModel()
                                    {
                                        PhotoPath = item?.PhotoPath,
                                        SendingChannel = item.SendingChannel,
                                        SendingText = item?.SendingText
                                    });
                                }
                                catch (Exception ex) { exception = ex.StackTrace; }

                                if (result)
                                {
                                    item.Status = "Успешно!";
                                    SuccessfullyRequested++;
                                    WaitingRequest--;
                                    DataWorker.AddInfoToDB(PhoneNumber, item.SendingChannel, String.IsNullOrEmpty(item?.PhotoPath),
                                                        item?.SendingText, item.DateTimeSending, true, exception);
                                }
                                else
                                {
                                    item.Status = "Не отправлено!";
                                    BadRequested++;
                                    WaitingRequest--;
                                    DataWorker.AddInfoToDB(PhoneNumber, item.SendingChannel, String.IsNullOrEmpty(item?.PhotoPath),
                                                        item?.SendingText, item.DateTimeSending, false, exception);
                                }
                                TelegramContext.LogOut();
                            }
                    }
                    Thread.Sleep(15000);
                }
            });
        }

        public void OnStopCommandExecute(object obj)
        {
            IsStartEnabled = true;
            IsStopEnabled = false;
            TeleStatus = "Рассылка остановлена!";
        }

        public void OnAddQueueCommandExecute(object obj)
        {
            if (!String.IsNullOrEmpty(Hours) && !String.IsNullOrEmpty(Minutes))
            {
                if (!String.IsNullOrEmpty(TextSending) || !String.IsNullOrEmpty(PhotoPath))
                {
                    int hour = Int32.Parse(Hours);
                    int minutes = Int32.Parse(Minutes);
              
                    DataGridQueries.Add(new DataGridQueryModel()
                    {
                        DateTimeSending = new DateTime(StartDateTime.Year, StartDateTime.Month, StartDateTime.Day, hour, minutes, 0),
                        PhotoPath = PhotoPath,
                        SendingChannel = MySelectedItem,
                        SendingText = TextSending,
                        Status = "Ожидание..."
                    });

                    PhotoPath = String.Empty;
                    TextSending = String.Empty;

                    WaitingRequest++;
                    TeleStatus = "Запись добавлена в \nочередь";
                }
                else
                    TeleStatus = "Добавьте текст\nили фото";
            }
            else
                TeleStatus = "Введите время";
        }

        public void OnChoosePhotoCommandExecute(object obj)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "jpg files (*.jpg)|*.jpg|png files (*.png)|*.png|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                PhotoPath = openFileDialog.FileName;
            }
        }

        public async void OnGetSummaryCommandExecute(object obj)
        {
            await Task.Run(() =>
            {
                string tempFile = "Svodka.txt";
                if (File.Exists(tempFile))
                {
                    File.Delete(tempFile);
                }

                var dtoResult = DataWorker.GetInfoFromDB();
                foreach (string item in dtoResult)
                {
                    File.AppendAllText(tempFile, item);
                }
            });

            TeleStatus = "Сводка создана";
        }

        public void OnClearPhotoPathCommandExecute(object obj)
        {
            PhotoPath = String.Empty;
        }
    }
}