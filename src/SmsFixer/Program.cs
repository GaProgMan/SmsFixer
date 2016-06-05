using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace SmsFixer
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            var smses = new Smses();

            // read the specific file from the run directory and seserialize
            // it's contents to an instance of the Smses object
            var serializer = new XmlSerializer(typeof(Smses));
            var stream = new FileStream("sms-20160220120000.xml", FileMode.Open);
            smses = (Smses) serializer.Deserialize(stream);

            // group the sms messages within the Smes object by Address;
            // pass the IGrouping into RemoveDuplicates;
            // order the returned data by date;
            // convert the IOrdered variable into an array;
            var nonDuplicated =
                RemoveDuplicates(smses.Sms.GroupBy(sm => sm.address)).OrderBy(mes => mes.date).ToArray();

            // replace the Sms array in the Smses object with the new array
            // (without duplicates);
            // update the count value of the Smses object
            smses.Sms = nonDuplicated;
            smses.count = $"{smses.Sms.Count()}";

            // Serialise the contents of the new Smses object to disk
            var writer = new XmlSerializer(typeof(Smses));
            var fs = File.Create("sms-20160220120000-noDuplicates.xml");
            writer.Serialize(fs, smses);
            fs.Close();
        }

        /// <summary>
        /// Used to instanciate a new Sms object array based on the contents of
        //  the passed in object (Enumerable of Sms objects, grouped by their
        //  address value) and return it
        /// </summary>
        static Sms[] RemoveDuplicates(IEnumerable<IGrouping<string, Sms>> groupedMessages )
        {
            var nonDuplicates = new List<Sms>();
            foreach (var grouped in groupedMessages)
            {
                // groupedMessages is an IEnumerable, so we need to group each
                // element in the IEnumerable by it's body text.
                var messageBodiesNoDuplicates = grouped.GroupBy(mes => mes.body);
                if (messageBodiesNoDuplicates.Any())
                {
                    // If there are any items in the new IGrouping (grouped), Threading
                    // we want to get all of the Key Value Pairs
                    foreach (var groupedduplicate in messageBodiesNoDuplicates.Select(kvp => kvp.ToList()))
                    {
                        // Since groupedduplicate contains an IEnumerable of
                        // Sms messages, grouped by their mesage body we can be
                        // sure that they are the same.
                        // Because of this, we only want the first in the list,
                        // regardless of the rest of it's data, so add it to the
                        // list of messages to return
                        nonDuplicates.Add(groupedduplicate.First());
                    }
                }
            }

            // return the list as an array
            // TODO: simplify this to return an IEnumberable or IList
            return nonDuplicates.ToArray();
        }
    }

    [XmlRoot("smses", Namespace = "", IsNullable = false)]
    public class Smses
    {
        // example smses entry
        //  <smses count="23274"
        //      backup_set="3ad6b18f-dcf7-4dfa-9ac5-4262d38ff805"
        //      backup_date="1455969600430">
        //          <sms ... content here... />
        //  </smses>
        [XmlAttribute]
        public string count { get; set; }
        [XmlAttribute]
        public string backup_set { get; set; }
        [XmlAttribute]
        public string backup_date { get; set; }
        [XmlElement("sms")]
        public Sms[] Sms { get; set; }

        //public Smses()
        //{
        //    Messages = new List<Sms>();
        //}
    }
    public class Sms
    {
        // example sms entry:
        //    <sms protocol="0"
        //      address="07123456789"
        //      date="1375363837534"
        //      type="2"
        //      subject="null"
        //      body="I think I found the penguin button! "
        //      toa="null"
        //      sc_toa="null"
        //      service_center="null"
        //      read="1"
        //      status="0"
        //      locked="0"
        //      date_sent="1375363865996"
        //      readable_date="1 Aug 2013 14:30:37"
        //      contact_name="Vincent Realperson" />
        [XmlAttribute]
        public string protocol { get; set; }
        [XmlAttribute]
        public string address { get; set; }
        [XmlAttribute]
        public string date { get; set; }
        [XmlAttribute]
        public string type { get; set; }
        [XmlAttribute]
        public string subject { get; set; }
        [XmlAttribute]
        public string body { get; set; }
        [XmlAttribute]
        public string toa { get; set; }
        [XmlAttribute]
        public string sc_toa { get; set; }
        [XmlAttribute]
        public string service_center { get; set; }
        [XmlAttribute]
        public string read { get; set; }
        [XmlAttribute]
        public string status { get; set; }
        [XmlAttribute]
        public string locked { get; set; }
        [XmlAttribute]
        public string date_sent { get; set; }
        [XmlAttribute]
        public string reabale_date { get; set; }
        [XmlAttribute]
        public string contact_name { get; set; }
    }
}
