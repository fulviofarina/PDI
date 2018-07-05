namespace PDILib
{
    public partial class Directory
    {
        public string[] files;
        public ImgDB.FilesDataTable table;
        public Directory(string path)
        {
            files = null;
            bool exists = System.IO.Directory.Exists(path);
            if (!exists) return;

            files = System.IO.Directory.GetFiles(path);

            if (table != null) table.Clear();
            else table = new ImgDB.FilesDataTable();

            foreach (string fi in files)
            {
                ImgDB.FilesRow f = table.NewFilesRow();
                f.Path = path + "\\";
                f.Filename = fi.Replace(f.Path, null);
                table.AddFilesRow(f);
            }
        }

    }

   

    

  

   
}
