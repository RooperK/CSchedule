using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Policy;

namespace CSchedule
{
    public class Schedule
    {
        [Serializable]
        private class Entry : IComparable
        {
            public string Name {get; set; }
            public string Content {get; set; }
            public DateTime DateAndTime { get;}

            public Entry(string name = null, string content = null, DateTime dateAndTime = default)
            {
                Name = name;
                Content = content;
                DateAndTime = dateAndTime;
            }

            public void Edit(string name, string content)
            {
                if (!string.IsNullOrWhiteSpace(name))
                {
                    Name = name;
                }
                if (!string.IsNullOrWhiteSpace(content))
                {
                    Content = content;
                }
            }

            public bool IsEmpty()
            {
                return string.IsNullOrWhiteSpace(Name) && string.IsNullOrWhiteSpace(Content);
            }

            public override int GetHashCode()
            {
                return DateAndTime.GetHashCode();
            }

            public int CompareTo(object obj)
            {
                return DateAndTime.CompareTo(((Entry) (obj)).DateAndTime);
            }

            public override string ToString()
            {
                string s = string.Format("{0}", DateAndTime.ToString("dd/MM/yyyy HH:mm"));
                if (Name != null) s += string.Format("\n{0}\n_____",Name);
                if (Content != null) s += string.Format("\n{0}",Content);;
                return s;
            }
        }
        
        private class Storage
        {
            public string _filePath = "temp.sch" ;

            public void Save(Entry[] entries)
            {
                using (var stream = File.Open(_filePath, FileMode.Create))
                {
                    var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                    binaryFormatter.Serialize(stream, entries);
                }
            }
            
            public Entry[] Load(string file)
            {
                try
                {
                    using (var stream = File.Open(string.Format("{0}.sch",file), FileMode.Open))
                    {
                        var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                        return (Entry[])binaryFormatter.Deserialize(stream);
                    
                    }  
                }
                catch (Exception e)
                {
                    return null;
                }
            }
        }
        
        private SortedDictionary<DateTime,Entry> Entries = new SortedDictionary<DateTime, Entry>();
        private Storage DB = new Storage();

        public void Load(string name)
        {
            Entry[] loaded;
            if ((loaded = DB.Load(name)) != null)
            {
                foreach (var entry in loaded)
                {
                    Entries.Add(entry.DateAndTime,entry);
                }
            }
        }

        public bool Add(string name, string content, DateTime dateTime)
        {
            if (!string.IsNullOrEmpty(name) || !string.IsNullOrEmpty(content))
            {
                Entry entry;
                if (!Entries.TryGetValue(dateTime, out entry))
                {
                    Entries.Add(dateTime,new Entry(string.IsNullOrEmpty(name) ? null : name,
                        string.IsNullOrEmpty(content) ? null : content, dateTime));
                    try
                    {
                        DB.Save(Entries.Values.ToArray());
                    }
                    catch (Exception e)
                    {
                        return true;
                    }
                }
                {
                    if (entry != null) entry.Content += string.Format("\n{0}",content);
                }
                
                return true;
            }
            return false;
        }
        public string Get(DateTime dateTime)
        {
            return Entries[dateTime].ToString();
        }
        public bool Edit(string name, string content, DateTime dateTime)
        {
            Entry entry;
            if (Entries.TryGetValue(dateTime,out entry))
            {
                entry.Edit(name,content);
            }

            return false;
        }
        public bool Remove(DateTime dateTime)
        {
            DB.Save(Entries.Values.ToArray());
            return Entries.Remove(dateTime);
        }
        public bool IsEmpty()
        {
            return Entries.Count == 0;
        }
        public string[] TextList()
        {
            string[] strings = new string[Entries.Count];
            int i = 0;
            foreach (var entry in Entries.Values)
            {
                strings[i] = entry.ToString();
                i++;
            }

            return strings;
        }
    }
}