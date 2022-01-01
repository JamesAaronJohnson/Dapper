using System;
using System.Text;

namespace Dapper
{
    public static class Dapper
    {
        /// <summary>ASCII Signature: DpR;</summary>
        private static readonly byte[] _Signature = { 0x44, 0x70, 0x52, 0x3B };
        /// <summary>The byte size of the Int32 appended data index.</summary>
        private const Int32 _WidthOffset = 4;

        public static bool HasAppendedData (byte[] sourceData)
        {
            throw new NotImplementedException ();
        }

        public static int GetDataIndex (byte[] sourceData)
        {
            if (sourceData == null)
                throw new ArgumentNullException (nameof (sourceData));

            throw new NotImplementedException ();
        }

        public static byte[] Append (byte[] sourceData, string messageData, Encoding encoding)
        {
            throw new NotImplementedException ();
        }

        public static byte[] Append (byte[] sourceData, byte[] newData)
        {
            if (sourceData == null)
                throw new ArgumentNullException ();

            throw new NotImplementedException ();
        }

        public static byte[] AppendAll (byte[] sourceData, string messageData, Encoding encoding)
        {
            throw new NotImplementedException ();
        }

        public static byte[] AppendAll (byte[] sourceData, byte[] newData)
        {
            throw new NotImplementedException ();
        }

        public static string Read (byte[] sourceData, Encoding encoding)
        {
            throw new NotImplementedException ();
        }

        public static byte[] Read (byte[] sourceData)
        {
            throw new NotImplementedException ();
        }
    }
}
