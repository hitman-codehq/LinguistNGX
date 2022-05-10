
using System;
using System.IO;
using System.ComponentModel;
//using System.Data.Linq;
//using System.Data.Linq.Mapping;
using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;
using Xamarin.Essentials;

// TODO: CAW - Not sure where the best place to put this is
namespace LinguistNGX.Services
{
    //[Table]
    public class Entry // : Object, INotifyPropertyChanged, INotifyPropertyChanging
    {
        //private int entryId;

        // Written: Sunday 28-Feb-2016 06:44 am, Code HQ Ehinger Tor

        /*public override bool Equals(Object obj)
        {
            bool retVal = false;
            Entry entry = (Entry) obj;

            // See if the foreign and English words in the current object match those in the Entry passed in
            //if ((entry != null) && (_foreign == entry.Foreign) && (_english == entry.English) && (Group.Name == entry.Group.Name))
            {
                retVal = true;
            }

            return retVal;
        }*/

        // Written: Sunday 28-Feb-2016 06:49 am, Code HQ Ehinger Tor

        /*public override int GetHashCode()
        {
            return RuntimeHelpers.GetHashCode(this);
        }*/

        //[Column(IsPrimaryKey = true, IsDbGenerated = true, DbType = "INT NOT NULL Identity", CanBeNull = false, AutoSync = AutoSync.OnInsert)]
        public int EntryId { get; set; }
        /*{
            get
            {
                return entryId;
            }

            set
            {
                if (entryId != value)
                {
                    NotifyPropertyChanging("EntryId");
                    entryId = value;
                    NotifyPropertyChanged("EntryId");
                }
            }
        }

        private string _foreign;*/

        //[Column]
        public string Foreign { get; set; }
        /*{
            get
            {
                return _foreign;
            }

            set
            {
                if (_foreign != value)
                {
                    NotifyPropertyChanging("Foreign");
                    _foreign = value;
                    NotifyPropertyChanged("Foreign");
                }
            }
        }

        private string _english;*/

        //[Column]
        public string English { get; set; }
        /*{
            get
            {
                return _english;
            }

            set
            {
                if (_english != value)
                {
                    NotifyPropertyChanging("English");
                    _english = value;
                    NotifyPropertyChanged("English");
                }
            }
        }

        //[Column]
        internal int _groupId;*/

        public int GroupId { get; set; }
        public Group Group { get; set; }

        /*private EntityRef<Group> _group;

        [Association(Storage = "_group", ThisKey = "_groupId", OtherKey = "Id", IsForeignKey = true)]
        public Group Group
        {
            get
            {
                return _group.Entity;
            }

            set
            {
                NotifyPropertyChanging("Group");
                _group.Entity = value;

                if (value != null)
                {
                    _groupId = value.Id;
                }

                NotifyPropertyChanged("Group");
            }
        }

        [Column(IsVersion = true)]
        private Binary _version;

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

        #region INotifyPropertyChanging Members

        public event PropertyChangingEventHandler PropertyChanging;

        private void NotifyPropertyChanging(string propertyName)
        {
            if (PropertyChanging != null)
            {
                PropertyChanging(this, new PropertyChangingEventArgs(propertyName));
            }
        }

        #endregion
        */
    }

    //[Table]
    public class Group //: INotifyPropertyChanged, INotifyPropertyChanging
    {
        // Written: Saturday 18-Oct-2014 12:00 pm, Code HQ Ehinger Tor

        /*public override bool Equals(Object obj)
        {
            bool retVal = false;
            Group group = (Group) obj;

            // See if the name of the current object matches the name of the Group passed in
            if ((group != null) && (_name == group.Name))
            {
                retVal = true;
            }

            return retVal;
        }*/

        // Written: Sunday 28-Feb-2016 07:48 am, Code HQ Ehinger Tor

        /*public override int GetHashCode()
        {
            return RuntimeHelpers.GetHashCode(this);
        }*/

        //private int _id;

        //[Column(DbType = "INT NOT NULL IDENTITY", IsDbGenerated = true, IsPrimaryKey = true)]
        public int GroupId { get; set; }
        /*{
            get
            {
                return _id;
            }

            set
            {
                NotifyPropertyChanging("Id");
                _id = value;
                NotifyPropertyChanged("Id");
            }
        }

        private string _name;*/

        //[Column]
        public string Name { get; set; }
        /*{
            get
            {
                return _name;
            }

            set
            {
                NotifyPropertyChanging("Name");
                _name = value;
                NotifyPropertyChanged("Name");
            }
        }*/

        /*private EntitySet<Entry> _entries;

        [Association(Storage = "_entries", OtherKey = "_groupId", ThisKey = "Id")]
        public EntitySet<Entry> Entries
        {
            get
            {
                return this._entries;
            }

            set
            {
                this._entries.Assign(value);
            }
        }

        public Group()
        {
            _entries = new EntitySet<Entry>(
                new Action<Entry>(this.attach_Entry),
                new Action<Entry>(this.detach_Entry)
                );
        }

        private void attach_Entry(Entry entry)
        {
            NotifyPropertyChanging("Entry");
            entry.Group = this;
        }

        private void detach_Entry(Entry entry)
        {
            NotifyPropertyChanging("Entry");
            entry.Group = null;
        }

        [Column(IsVersion = true)]
        private Binary _version;

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

        #region INotifyPropertyChanging Members

        public event PropertyChangingEventHandler PropertyChanging;

        private void NotifyPropertyChanging(string propertyName)
        {
            if (PropertyChanging != null)
            {
                PropertyChanging(this, new PropertyChangingEventArgs(propertyName));
            }
        }

        #endregion
        */
    }

    public class DatabaseContext : DbContext
    {
        private string DatabaseName;
        public DbSet<Group> Groups { get; set; }
        public DbSet<Entry> Entries { get; set; }

        public DatabaseContext(string connectionString)
        {
            DatabaseName = connectionString;

            SQLitePCL.Batteries_V2.Init();
            this.Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string dbPath = Path.Combine(FileSystem.AppDataDirectory, DatabaseName);

            optionsBuilder.UseSqlite($"FileName={dbPath}");
        }

        // TODO: CAW - Why is this file mixed CR/LF?
        static public bool DatabaseExists(string connectionString)
        {
            return File.Exists(Path.Combine(FileSystem.AppDataDirectory, connectionString));
        }
    }
}
