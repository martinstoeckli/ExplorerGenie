// Copyright © 2018 Martin Stoeckli.
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace ExplorerGenieShared
{
    /// <summary>
    /// Helper class for serializing models to XML and back.
    /// </summary>
    public class XmlUtils
    {
        /// <summary>
        /// Uses the DotNet serialization framework to serialize an object to a stream.
        /// The object should be tagged with the [Xml...] attributes. Make sure, that only valid
        /// strings are serialized, test is with <see cref="SanitizeXmlString"/> if necessary.
        /// </summary>
        /// <param name="obj">Object to serialize.</param>
        /// <param name="outputStream">The xml is written to this stream.
        /// The stream is closed automatically.</param>
        /// <param name="encoding">The optional encoding, which is used for writing. The default
        /// encoding is "utf-8"</param>
        /// <param name="omitNamespaces">If the namespace xmlns:xsi and xmlns:xsd should not be
        /// written, set this parameter to true.</param>
        public static void SerializeToXmlStream(object obj, Stream outputStream, Encoding encoding = null, bool omitNamespaces = false)
        {
            if (encoding == null)
                encoding = Encoding.UTF8;

            XmlWriterSettings settings = new XmlWriterSettings
            {
                CheckCharacters = false,
                Indent = true,
                Encoding = encoding
            };

            using (XmlWriter xmlWriter = XmlWriter.Create(outputStream, settings))
            {
                XmlSerializer serializer = new XmlSerializer(obj.GetType());
                XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
                if (omitNamespaces)
                    namespaces.Add(string.Empty, string.Empty);
                serializer.Serialize(xmlWriter, obj, namespaces);
            }
        }

        /// <summary>
        /// Uses the DotNet serialization framework to serialize an object to an array of bytes.
        /// The object should be tagged with the [Xml...] attributes.
        /// </summary>
        /// <param name="obj">Object to serialize.</param>
        /// <param name="encoding">The optional encoding, which is used for writing. The default
        /// encoding is "utf-8"</param>
        /// <param name="omitNamespaces">If the namespace xmlns:xsi and xmlns:xsd should not be
        /// written, set this parameter to true.</param>
        /// <returns>A new created byte array, containing the serialized object.</returns>
        public static byte[] SerializeToXmlBytes(object obj, Encoding encoding = null, bool omitNamespaces = false)
        {
            byte[] result;
            using (MemoryStream stream = new MemoryStream())
            {
                SerializeToXmlStream(obj, stream, encoding, omitNamespaces);
                result = stream.ToArray();
            }
            return result;
        }

        /// <summary>
        /// Mainly used for unit-testing, it uses the DotNet serialization framework to serialize
        /// an object to an XDocument.
        /// </summary>
        /// <param name="obj">Object to serialize.</param>
        /// <returns>Serialized XDocument</returns>
        internal static XDocument SerializeToXmlDocument(object obj)
        {
            XDocument result = new XDocument();
            using (XmlWriter xmlWriter = result.CreateWriter())
            {
                XmlSerializer serializer = new XmlSerializer(obj.GetType());
                serializer.Serialize(xmlWriter, obj);
            }
            return result;
        }

        /// <summary>
        /// Mainly used for unit-testing, it uses the DotNet serialization framework to serialize
        /// an object to a string. Because strings are always unicode, the encoding "utf-16" is
        /// used. If you want to store the xml to a file, please use <see cref="SerializeToXmlStream(object, Stream, Encoding)"/>
        /// instead, so the encoding can be set correctly.
        /// </summary>
        /// <param name="obj">Object to serialize.</param>
        /// <param name="omitNamespaces">If the namespace xmlns:xsi and xmlns:xsd should not be
        /// written, set this parameter to true.</param>
        /// <returns>String containing the Xml of the serialized object.</returns>
        internal static string SerializeToString(object obj, bool omitNamespaces = false)
        {
            Encoding encoding = Encoding.Unicode;
            byte[] bytes = SerializeToXmlBytes(obj, encoding, omitNamespaces);
            return encoding.GetString(bytes);
        }

        /// <summary>
        /// Creates an XDocument from an array of bytes, containing the XML content.
        /// </summary>
        /// <param name="bytes">Xml file content as byte array.</param>
        /// <returns>Loaded XDocument.</returns>
        public static XDocument LoadFromXmlBytes(byte[] bytes)
        {
            XDocument result;
            using (MemoryStream stream = new MemoryStream(bytes))
            {
                result = XDocument.Load(stream);
            }
            return result;
        }

        /// <summary>
        /// Creates an XDocument from an string, containing the XML content.
        /// </summary>
        /// <param name="xml">Xml file content as string.</param>
        /// <returns>Loaded XDocument.</returns>
        public static XDocument LoadFromString(string xml)
        {
            XDocument result;
            using (MemoryStream stream = new MemoryStream(Encoding.Unicode.GetBytes(xml)))
            {
                result = XDocument.Load(stream);
            }
            return result;
        }

        /// <summary>
        /// Uses the DotNet serialization framework to deserialize an object from an XML document.
        /// The object should be tagged with the [Xml...] attributes.
        /// </summary>
        /// <typeparam name="T">Type of the object to create.</typeparam>
        /// <param name="xml">Read the xml from this XML document.</param>
        /// <returns>New deserialized object.</returns>
        public static T DeserializeFromXmlDocument<T>(XDocument xml)
        {
            T result;
            using (XmlReader xmlReader = xml.CreateReader())
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                result = (T)serializer.Deserialize(xmlReader);
            }
            return result;
        }
    }
}
