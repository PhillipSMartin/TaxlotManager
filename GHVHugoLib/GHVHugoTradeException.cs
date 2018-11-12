using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace GHVHugoLib
{
    [Serializable]
    public class GHVHugoTradeException : GHVHugoException
    {
        private int m_rc;

        public GHVHugoTradeException() : this(0) { }
        public GHVHugoTradeException(int rc) : base(TranslateReturnCode(rc)) { m_rc = rc; }
        public GHVHugoTradeException(string message) : base(message) { }
        public GHVHugoTradeException(string message, Exception inner) : base(message, inner) { }
        protected GHVHugoTradeException(SerializationInfo info, StreamingContext ctx) : base(info, ctx) { }

        public int ReturnCode { get { return m_rc; } }
        public const int CannotAddOption = 18;
        private static string TranslateReturnCode(int rc)
        {
            switch (rc)
            {
                case 0:
                    return String.Empty;
                case 1:
                    return "Underlying name not found";
                case 2:
                    return "Stock id not found";
                case 3:
                    return "Series id not found";
                case 4:
                    return "Invalid exercise type";
                case 5:
                    return "Subacct not found";
                case 6:
                    return "Invalid option type";
                case 7:
                    return "Invalid trade type";
                case 8:
                    return "Industry group not found";
                case 9:
                    return "Exchange not found";
                case 10:
                    return "Invalid underlying type";
                case 11:
                    return "Invalid account type";
                case 12:
                    return "Underlying id not found";
                case 13:
                    return "Illegal operation";
                case 14:
                    return "Invalid contra";
                case 15:
                    return "Invalid billing type";
                case 16:
                    return "Cannot enter a trade before import is done";
                case 17:
                    return "Cannot update a reconciled trade";
                case 18:
                    return "Unable to add new option to Hugo";
                case 19:
                    return "Invalid trade medium";
                default:
                    return String.Format("Undocumented error - rc={0}", rc);
            }
        }
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("ReturnCode", m_rc);
        }
    }
}
