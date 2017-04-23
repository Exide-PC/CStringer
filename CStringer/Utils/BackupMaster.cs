using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace CStringer.Utils
{
    public class BackupMaster
    {
        public string FullPath { get; private set; } = null;
        private string fileName = null;
        private string fileExtension = null;
        private string parentDir = null;
        public int MaxCount { get; private set; }
        private Regex backupPattern;

        public BackupMaster(string filePath, int maxCount = 5)
        {
            this.FullPath = filePath;
            this.SetMaxCount(maxCount);

            FileInfo backupInfo = new FileInfo(filePath);
            this.fileExtension = backupInfo.Extension;
            this.fileName = backupInfo.Name.Replace(fileExtension, "");
            this.parentDir = Directory.GetParent(filePath).ToString();

            this.backupPattern = new Regex($"{fileName}_backup[0-9]+\\{fileExtension}", RegexOptions.Compiled);

            TrimCount(this.MaxCount);
        }

        public int NextFreeIndex
        {
            get
            {
                int count = 0;

                while (File.Exists(GenBackupPath(count + 1)))
                    count++;

                return count;
            }
        }

        public void DeleteBackup(string backupPath)
        {
            if (File.Exists(backupPath))
                File.Delete(backupPath);
            else throw new FileNotFoundException("File doesn't exist.");

            TrimCount(MaxCount);
        }

        public void SetMaxCount(int maxCount)
        {
            int oldMaxCount = this.MaxCount;

            if (maxCount > 0)
                this.MaxCount = maxCount;
            else if (maxCount == 0)
                this.MaxCount = int.MaxValue;            
            else throw new ArgumentException("Count can't be less than zero.");

            if (this.MaxCount < oldMaxCount)
                TrimCount(MaxCount);
        }

        public void TrimCount(int count)
        {
            // Using queue of backups to optimize this method
            Queue<string> backupQueue = new Queue<string>();

            // Filling the queue from backups array
            foreach (string backup in this.Backups)
                backupQueue.Enqueue(backup);
                        
            // Deleting all old extra backups
            while (backupQueue.Count > count)
                File.Delete(backupQueue.Dequeue());
            

            int queueCount = backupQueue.Count;

            // Rebuilding backups structure
            for (int i = 1; i <= queueCount; i++)
            {
                string sourceBackupPath = backupQueue.Dequeue();
                string targetBackupPath = GenBackupPath(i);

                if (sourceBackupPath != targetBackupPath)
                {
                    DateTime creationTime = File.GetCreationTime(sourceBackupPath);
                    File.Move(sourceBackupPath, targetBackupPath);
                    File.SetCreationTime(targetBackupPath, creationTime);
                }                    
            }
        }

        public string[] Backups
        {
            get
            {
                List<string> backups = new List<string>();

                var files = Directory.EnumerateFiles(parentDir);

                foreach (string file in files)
                {
                    string name = file.Split('\\').Last();

                    //if (name.StartsWith($"{this.fileName}_backup"))
                    if (this.backupPattern.IsMatch(name))
                        backups.Add(file);
                }

                return backups.ToArray();
            }
        }

        public void CreateBackup()
        {
            // trimming backups to create new backup correctly
            this.TrimCount(MaxCount - 1);

            string targetPath = GenBackupPath(this.NextFreeIndex + 1);
            File.Copy(FullPath, targetPath);
            File.SetCreationTime(targetPath, DateTime.Now);
        }

        public void RestoreBackup(string sourceBackupPath)
        {
            if (sourceBackupPath == null)
                throw new ArgumentNullException("Source backup path is null.");

            File.Copy(sourceBackupPath, FullPath, overwrite: true);
            this.TrimCount(this.MaxCount);
        }

        public void RestoreBackup()
        {
            RestoreBackup(Backups?.Last());
        }

        public void DeleteAll()
        {
            string[] backupsPaths = this.Backups;

            foreach (string backup in backupsPaths)
                File.Delete(backup);
        }

        public string GenBackupPath(int id)
        {
            return string.Format($@"{parentDir}\{fileName}_backup{id}{fileExtension}");
        }
    }
}
