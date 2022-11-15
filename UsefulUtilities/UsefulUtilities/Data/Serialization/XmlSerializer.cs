using System;
using System.IO;
using rm = UsefulUtilities.Resources.Data.Xml.Serializer;

namespace UsefulUtilities.Data.Serialization
{
    public static class XmlSerializer
    {
        /// <summary>
        /// Serialize data to file path
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="serializable"></param>
        /// <param name="file"></param>
        public static void SerializeToFile<T>(T serializable, string file) where T : class
        {
            FileInfo destination = new FileInfo(file);
            if (!destination.Directory.Exists) { destination.Directory.Create(); }

            using (StreamWriter sw = new StreamWriter(destination.FullName, false))
            {
                string xml = Serialize<T>(serializable);
                sw.WriteLine(xml);
            }
        }

        /// <summary>
        /// Serializes the specified serializable.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="serializable">The serializable.</param>
        /// <returns></returns>
        /// <exception cref="NotSupportedException">thrown if object is not marked Serializable().</exception>
        public static string Serialize<T>(T serializable) where T : class
        {
            string finalxml = "";
            // Verify object is marked serializable
            if (typeof(T).IsSerializable)
            {
                // Copy so original object isn't modified
                object mcopy = Clone(serializable);

                using (MemoryStream ms = new MemoryStream())
                {
                    // Settings to format xml
                    System.Xml.XmlWriterSettings settings = new System.Xml.XmlWriterSettings() { OmitXmlDeclaration = true, Indent = true };
                    // Create emtpty namespaces
                    System.Xml.Serialization.XmlSerializerNamespaces ns = new System.Xml.Serialization.XmlSerializerNamespaces();
                    ns.Add(string.Empty, string.Empty);
                    // Create writer and serializer
                    System.Xml.XmlWriter xw = System.Xml.XmlWriter.Create(ms, settings);
                    System.Xml.Serialization.XmlSerializer xs = new System.Xml.Serialization.XmlSerializer(typeof(T));
                    // Serialize object
                    xs.Serialize(xw, mcopy, ns);
                    // Reset memorystream and read results
                    ms.Position = 0;
                    using (StreamReader sr = new StreamReader(ms))
                    {
                        finalxml = sr.ReadToEnd();
                    }
                }
                // Clean deep copy if implements IDisposable
                if (typeof(IDisposable).IsAssignableFrom(typeof(T))) { ((IDisposable)mcopy).Dispose(); }
                mcopy = null;
            }
            else
            {
                throw new NotSupportedException(rm.notSerializable);
            }
            return finalxml;
        }

        /// <summary>
        /// Deserialize xml string from file path
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sourceFile"></param>
        /// <param name="IsFileLoc"></param>
        /// <returns></returns>
        public static T DeSerializeFromFile<T>(string sourceFile) where T : class
        {
            if (File.Exists(sourceFile))
            {
                string xml = File.ReadAllText(sourceFile);
                return DeSerialize<T>(xml);
            }
            else
            {
                throw new Exception($"{rm.sourceFileDNE}{Environment.NewLine}{sourceFile}");
            }
        }

        /// <summary>
        /// Deserialize xml string
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xmlstring">The xmlstring.</param>
        /// <returns></returns>
        public static T DeSerialize<T>(string xmlstring)
        {
            object retitm = null;
            using (StringReader sr = new StringReader(xmlstring))
            {
                using (System.Xml.XmlTextReader xr = new System.Xml.XmlTextReader(sr))
                {
                    // Ignore namespaces on deserialization
                    xr.Namespaces = false;
                    // Deserialize information from xml text reader
                    System.Xml.Serialization.XmlSerializer xs = new System.Xml.Serialization.XmlSerializer(typeof(T));
                    retitm = xs.Deserialize(xr);
                }
            }
            return (T)retitm;
        }

        /// <summary> 
        /// Perform a deep copy of the object. 
        /// </summary> 
        /// <typeparam name="T">The type of object being copied.</typeparam> 
        /// <param name="source">The object instance to copy.</param> 
        /// <returns>The copied object.</returns> 
        private static T Clone<T>(T source)
        {
            // Verify object is serializable and not null
            if (Object.ReferenceEquals(source, null)) { return default(T); }
            // Create deep copy of object
            System.Xml.Serialization.XmlSerializer formatter = new System.Xml.Serialization.XmlSerializer(source.GetType());
            using (MemoryStream ms = new MemoryStream())
            {
                formatter.Serialize(ms, source);
                ms.Position = 0;
                return (T)formatter.Deserialize(ms);
            }
        }
    }
}
