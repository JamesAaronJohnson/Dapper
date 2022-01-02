using System;
using System.Text;

namespace Dapper
{
    /// <summary>Utility class for appending and reading appended data from the end of a file.</summary>
    public static class Dapper
    {
        /// <summary>ASCII Signature: DpR;</summary>
        private static readonly byte[] _Signature = { 0x44, 0x70, 0x52, 0x3B };
        /// <summary>The byte size of the Int32 appended data index.</summary>
        private const Int32 _WidthOffset = 4;

        /// <summary>Determines if the given data has appended data at the end to extract.</summary>
        /// <param name="sourceData">The data to check.</param>
        /// <returns>True if appended data is found, False if not.</returns>
        /// <exception cref="ArgumentNullException"></exception>
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

        /// <summary>Retrieve the start index of the appended data from the signature.</summary>
        /// <param name="sourceData">The data to extract the index from.</param>
        /// <returns>An Int32 index marking the start of the appended data.</returns>
        /// <exception cref="ArgumentNullException"></exception>
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

        /// <summary>Appends extra data to the end of the file. If any previous data exists, adds onto it and updates the signature to match.</summary>
        /// <param name="sourceData">The data to append onto.</param>
        /// <param name="messageData">The string data to append.</param>
        /// <param name="encoding">The character encoding to write as.</param>
        /// <returns>The original data with the new data appended to the end.</returns>
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

        /// <summary>Appends extra data to the end of the file. If any previous data exists, adds onto  it and updates the signature to match.</summary>
        /// <param name="sourceData">The data to append onto.</param>
        /// <param name="newData">The new data to append.</param>
        /// <returns>The original data with the new data appended to the end.</returns>
        /// <exception cref="ArgumentNullException"></exception>
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

        /// <summary>Appends data to the end of the file. If any previous data exists, it's lost and replaced with the new data.</summary>
        /// <param name="sourceData">The data to append onto.</param>
        /// <param name="messageData">The string data to append.</param>
        /// <param name="encoding">The character encoding to write as.</param>
        /// <returns>The original data with the new data appended to the end.</returns>
        /// <exception cref="ArgumentNullException"></exception>
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

        /// <summary>Appends data to the end of the file. If any previous data exists, it's lost and replaced with the new data.</summary>
        /// <param name="sourceData">The data to append onto.</param>
        /// <param name="newData">The new data to append./</param>
        /// <returns>The original data with the new data appended to the end.</returns>
        /// <exception cref="ArgumentNullException"></exception>
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

        /// <summary>Appends data onto a given source at the specified starting position.</summary>
        /// <param name="sourceData">The data to append onto.</param>
        /// <param name="newData">The new data to append.</param>
        /// <param name="startIndex">The starting index of the appended data.</param>
        /// <returns>The original data with the new data appended. </returns>
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

        /// <summary>Read appended data and return a string represention in the specified encoding.</summary>
        /// <param name="sourceData">The data to extract appended data from.</param>
        /// <param name="encoding">The character encoding to read as.</param>
        /// <returns>A string representation of the appended data.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static string Read (byte[] sourceData, Encoding encoding)
        {
            if (sourceData == null)
                throw new ArgumentNullException (nameof (sourceData));

            if (encoding == null)
                throw new ArgumentNullException (nameof (encoding));

            byte[] appendedData = Read (sourceData);
            return encoding.GetString (appendedData);
        }

        /// <summary>Read appended data from the given data.</summary>
        /// <param name="sourceData">The data to read from.</param>
        /// <returns>The extracted data.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static byte[] Read (byte[] sourceData)
        {
            if (sourceData == null)
                throw new ArgumentNullException (nameof (sourceData));

            int dataIndex = GetDataIndex (sourceData);

            // Create a destination for the appended data.
            byte[] appendedData = new byte[sourceData.Length - dataIndex - _Signature.Length - _WidthOffset];

            // Extract the data from the appended index location.
            for (int i = 0; i < appendedData.Length; i++)
            {
                appendedData[i] = sourceData[dataIndex + i];
            }

            return appendedData;
        }
    }
}
