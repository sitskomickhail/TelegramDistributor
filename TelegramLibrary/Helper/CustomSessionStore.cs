using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLSharp.Core;

namespace TelegramLibrary.Helper
{
    public class CustomSessionStore : ISessionStore
    {
        private string _nuberPhone;

        public CustomSessionStore(string nuberPhone)
        {
            _nuberPhone = nuberPhone;
        }
        public void Save(Session session)
        {
            var dir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Sessions");

            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            var file = Path.Combine(dir, _nuberPhone + ".dat");

            using (FileStream fileStream = new FileStream(string.Format(file, (object)session.SessionUserId), FileMode.OpenOrCreate))
            {
                byte[] bytes = session.ToBytes();
                fileStream.Write(bytes, 0, bytes.Length);
            }
        }

        public Session Load(string sessionUserId)
        {
            var dir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Sessions");

            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            var file = Path.Combine(dir, _nuberPhone + ".dat");

            string path = string.Format(file, (object)sessionUserId);
            if (!File.Exists(path))
                return (Session)null;

            var buffer = File.ReadAllBytes(path);
            return Session.FromBytes(buffer, this, sessionUserId);
        }
    }
}
