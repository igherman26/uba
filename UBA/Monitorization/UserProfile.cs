using System.IO;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using System;

namespace UBA
{
    public class UserProfile
    {
        public static string profilesFolder = ".\\profiles";

        public string username;

        public string profileLocation;
        public string profileDataLocation;
        public string basicDataLocation;
        public string baselineDataLocation;

        public List<string> availableDates;
        public List<string> baselineDates;

        public UserProfile(string username)
        {
            this.username = username;
            profileLocation = Path.Combine(profilesFolder, username);
            basicDataLocation = Path.Combine(profileLocation, "basic.json");
            baselineDataLocation = Path.Combine(profileLocation, "baseline.json");
            profileDataLocation = Path.Combine(profileLocation, "profile.json");
            availableDates = new List<string>();
            baselineDates = new List<string>();
        }

        // save the profile data
        public bool SaveProfile()
        {
            try
            {
                File.WriteAllText(profileDataLocation, JsonConvert.SerializeObject(this));
                return true;
            }
            catch
            {
                return false;
            }
        }

        // load the user profile: basic data, baseline data and actual data if they exist
        public static UserProfile LoadProfile(string username)
        {
            // check if the profile exists
            if (!ExistingProfiles().Contains(username))
                return null;

            UserProfile up;

            try
                {
                using (StreamReader r = new StreamReader(Path.Combine(profilesFolder, username, "profile.json")))
                {
                    string json = r.ReadToEnd();
                    up = JsonConvert.DeserializeObject<UserProfile>(json);
                }
                return up;
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        // verify profilesFolder for profiles and return an array of them
        public static List<string> ExistingProfiles()
        {
            // create profiles folder if it doesn't exist
            Directory.CreateDirectory(profilesFolder);

            string[] dirs = Directory.GetDirectories(profilesFolder);
            List<string> retStrs = new List<string>();

            foreach (string s in dirs)
                retStrs.Add(s.Remove(0, profilesFolder.Length + 1));

            return retStrs;
        }

        // create a new profile
        public static bool CreateNewProfile(string username)
        {
            // check if it already exists(the folder)
            if (ExistingProfiles().Contains(username))
                return false;

            // create the folder
            Directory.CreateDirectory(Path.Combine(profilesFolder, username));

            // create object and save it
            UserProfile up = new UserProfile(username);
            if (!up.SaveProfile())
                return false;

            return true;
        }
    }
}
