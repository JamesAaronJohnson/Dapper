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
            if (sourceData == null)
                throw new ArgumentNullException (nameof (sourceData));

            // Check to ensure there is enough space to contain appended data.
            if (sourceData.Length <= _Signature.Length + _WidthOffset)
                return false;

            // Grab the starting index of the signature and the data index.
            int signatureIndex = sourceData.Length - ( _Signature.Length + _WidthOffset );

            // Ensure that there is a signature and an int defining the start location of the appended data.
            if (( sourceData.Length - signatureIndex ) < _Signature.Length + _WidthOffset)
                return false;

            // Verify there is a valid signature of appended data.
            for (int i = 0; i < _Signature.Length; i++)
            {
                if (sourceData[signatureIndex + i] != _Signature[i])
                    return false;
            }

            // Get the starting index of the appended data from the signature.
            int dataIndex = BitConverter.ToInt32 (sourceData, signatureIndex + _Signature.Length);

            // Ensure the index of the appended data is a valid position within the source data.
            if (dataIndex >= sourceData.Length || dataIndex <= 0)
                return false;

            // All checks have passed, there is likely appended data to be found.
            return true;
        }

        public static int GetDataIndex (byte[] sourceData)
        {
            if (sourceData == null)
                throw new ArgumentNullException (nameof (sourceData));

            // Extract the index of the appended data from the signature.
            byte[] dataIndexBytes = new byte[_WidthOffset];
            for (int i = 0; i < dataIndexBytes.Length; i++)
            {
                dataIndexBytes[i] = sourceData[sourceData.Length - _WidthOffset + i];
            }

            return BitConverter.ToInt32 (dataIndexBytes);
        }

        public static byte[] Append (byte[] sourceData, string messageData, Encoding encoding)
        {
            if (sourceData == null)
                throw new ArgumentNullException (nameof (sourceData));

            if (messageData == null)
                throw new ArgumentNullException (nameof (messageData));

            if (encoding == null)
                throw new ArgumentNullException (nameof (encoding));

            byte[] data = encoding.GetBytes (messageData);
            return Append (sourceData, data);
        }

        public static byte[] Append (byte[] sourceData, byte[] newData)
        {
            if (sourceData == null)
                throw new ArgumentNullException (nameof (sourceData));

            if (newData == null)
                throw new ArgumentNullException (nameof (newData));

            // Write new data after the old appended data and update signature to match.
            if (HasAppendedData (sourceData))
            {
                int startIndex = sourceData.Length - ( _Signature.Length + _WidthOffset );
                return AppendData (sourceData, newData, startIndex);
            }

            // No data is appended yet, so start it from the end of this data.
            return AppendData (sourceData, newData, sourceData.Length);
        }

        public static byte[] AppendAll (byte[] sourceData, string messageData, Encoding encoding)
        {
            if (sourceData == null)
                throw new ArgumentNullException (nameof (sourceData));

            if (messageData == null)
                throw new ArgumentNullException (nameof (messageData));

            if (encoding == null)
                throw new ArgumentNullException (nameof (encoding));

            byte[] data = encoding.GetBytes (messageData);
            return AppendAll (sourceData, data);
        }

        public static byte[] AppendAll (byte[] sourceData, byte[] newData)
        {
            if (sourceData == null)
                throw new ArgumentNullException (nameof (sourceData));

            if (newData == null)
                throw new ArgumentNullException (nameof (newData));

            // Write new data while ignoring old appended data.
            if (HasAppendedData (sourceData))
            {
                int startIndex = GetDataIndex (sourceData);
                return AppendData (sourceData, newData, startIndex);
            }

            // No data is appended yet, so start it from the end of this data.
            return AppendData (sourceData, newData, sourceData.Length);
        }

        private static byte[] AppendData (byte[] sourceData, byte[] newData, int startIndex)
        {
            byte[] dataIndex = BitConverter.GetBytes (startIndex);
            byte[] appendedData = new byte[startIndex + newData.Length + _Signature.Length + _WidthOffset];

            // Copy the source data into the new byte array, ignoring old appended data.
            Array.Copy (sourceData, 0, appendedData, 0, startIndex);
            // Copy the new data in the new byte array.
            Array.Copy (newData, 0, appendedData, startIndex, newData.Length);
            // Copy the signature after the appended data.
            Array.Copy (_Signature, 0, appendedData, startIndex + newData.Length, _Signature.Length);
            // Copy the index of the appended data.
            Array.Copy (dataIndex, 0, appendedData, startIndex + newData.Length + _Signature.Length, dataIndex.Length);

            return appendedData;
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
