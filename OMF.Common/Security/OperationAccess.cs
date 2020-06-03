using OMF.Common.Cryptography;
using System;

namespace OMF.Common.Security
{
    public class OperationAccess : IOperationAccess, ICloneable
    {
        public static string MessagePattern = "محدودیت دسترسی: {0}";

        public static bool PasswordEqual(string hashedPassword, string password)
        {
            if (string.IsNullOrWhiteSpace(hashedPassword) && string.IsNullOrWhiteSpace(password) || !string.IsNullOrWhiteSpace(hashedPassword) && string.IsNullOrWhiteSpace(password))
                return false;
            if (password.Length > 40)
                return hashedPassword == password;
            return CryptoHelper.VerifyHash(password, "SHA256", hashedPassword);
        }

        public object Clone()
        {
            return (object)new OperationAccess()
            {
                CanDelete = this.CanDelete,
                CanUpdate = this.CanUpdate,
                CanInsert = this.CanInsert,
                CanImport = this.CanImport,
                CanView = this.CanView,
                CanExport = this.CanExport,
                CanPrint = this.CanPrint
            };
        }

        public bool CanInsert { get; set; }

        public bool CanView { get; set; }

        public bool CanUpdate { get; set; }

        public bool CanDelete { get; set; }

        public bool CanExport { get; set; }

        public bool CanPrint { get; set; }

        public bool CanImport { get; set; }
    }
}
