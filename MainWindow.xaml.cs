using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Newtonsoft.Json;
using static homerworker.MainWindow;
using Path = System.IO.Path;

namespace homerworker
{
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MainWindow : Window
    {
        List<string> hw = new List<string>();
        List<string> student = new List<string>();
        List<string> hwDocument = new List<string>();
        List<Student_Metadata> StudentMetadata = new List<Student_Metadata>();
        public MainWindow()
        {
            InitializeComponent();
            HwListParse();
            // MetadataWrite();
            
            MetadataLoader();
        }
        private void CheckPath()
        {

        }
        private void HwListParse()
        {
            string _path = homerworker.Properties.Settings.Default.directoryPath;
            hwSlection_lb.SelectionMode = System.Windows.Controls.SelectionMode.Single;

            string[] hwnames= Directory.GetDirectories(_path);
            if (hwnames.Length!=0)
            {
                foreach (string item in hwnames)
                {
                    hw.Add(System.IO.Path.GetFileName(item));
                    hwSlection_lb.Items.Add(System.IO.Path.GetFileName(item));
                }
            }
                       
        }

        private Student_Metadata MetadataSeek(string StudentID,string HomeworkName)
        {

            if (StudentMetadata == null)
            {
                StudentMetadata = new List<Student_Metadata>();
            }
            bool foundStudent = false;
            foreach (Student_Metadata singleStudent in StudentMetadata)
            {
                if (singleStudent.Student == StudentID)
                {

                    //student found!
                    foundStudent = !foundStudent;
                    return singleStudent;

                }
            }
            
            return null;

        }
        private void MetadataLoader()
        {


            //string raw= "[{\"Student\":\"109423069\",\"Hw_Metadatas\":[{\"Title\":\"1-2\",\"Grade\":7878},{\"Title\":\"1-3\",\"Grade\":7878},{\"Title\":\"1-1\",\"Grade\":7878}]},{\"Student\":\"109423070\",\"Hw_Metadatas\":[{\"Title\":\"1-1\",\"Grade\":7878},{\"Title\":\"1-2\",\"Grade\":7878},{\"Title\":\"1-3\",\"Grade\":7878}]}]";


            //Load metadata from outside source into StudentMetadata list.
            string raw = "";
            string metadataPath = homerworker.Properties.Settings.Default.directoryPath;
            using (StreamReader reader=new StreamReader(System.IO.Path.Combine(metadataPath, "metadata")))
            {
                raw = reader.ReadToEnd();
            }


           StudentMetadata = JsonConvert.DeserializeObject<List<Student_Metadata>>(raw);




        }
        private void MetadataWrite(string StudentID,string HomeworkName,int Score)
        {

            bool foundStudent = false;

            if (StudentMetadata==null)
            {
                StudentMetadata = new List<Student_Metadata>();
            }

            foreach (Student_Metadata singleStudent in StudentMetadata)
            {
                if (singleStudent.Student ==StudentID)
                {

                    //student found!
                    foundStudent=!foundStudent;
                    bool foundHomework = false;
                    foreach (Hw_Metadata singleHomework in singleStudent.Hw_Metadatas)
                    {
                        if (singleHomework.Title==HomeworkName)
                        {
                            //homework found!
                            foundHomework = !foundHomework;
                            singleHomework.Score = Score;

                        }
                    }
                    if (!foundHomework)
                    {
                        //add a new homework metadat due to no existed found.
                        Hw_Metadata newHomework = new Hw_Metadata
                        {
                         Title=HomeworkName,Score=Score                       
                        };
                        singleStudent.Hw_Metadatas.Add(newHomework);
                    }

                }
            }
            if (!foundStudent)
            {
                //no existed student found add one.
                Hw_Metadata newHomework = new Hw_Metadata
                {
                    Title = HomeworkName,
                    Score = Score
                };

                Student_Metadata newStudent = new Student_Metadata
                {
                    Student = StudentID,
                    
                };
                newStudent.Hw_Metadatas.Add(newHomework);

                StudentMetadata.Add(newStudent);

            }


            string json = JsonConvert.SerializeObject(StudentMetadata);


            string metadataPath=homerworker.Properties.Settings.Default.directoryPath;
            using (StreamWriter writer = new StreamWriter(System.IO.Path.Combine(metadataPath, "metadata")))
            {
                writer.Write(json);
            }



            //manual add a new 
            //List<Student_Metadata> student_s = new List<Student_Metadata>();

            //Student_Metadata sm = new Student_Metadata
            //{
            //    Student="109423069",
            //    Hw_Metadatas=new List<Hw_Metadata>
            //    {
            //        new Hw_Metadata{Title="1-2",Score=7878} ,
            //        new Hw_Metadata{Title="1-3",Score=7878},
            //        new Hw_Metadata{Title="1-1",Score=7878}
            //    }

            //};
            //Student_Metadata sm1 = new Student_Metadata
            //{
            //    Student = "109423070",
            //    Hw_Metadatas = new List<Hw_Metadata>
            //    {
            //        new Hw_Metadata{Title="1-1",Score=7878} ,
            //        new Hw_Metadata{Title="1-2",Score=7878},
            //        new Hw_Metadata{Title="1-3",Score=7878}
            //    }

            //};

            //student_s.Add(sm);
            //student_s.Add(sm1);

            //string json = JsonConvert.SerializeObject(student_s);
            
        }
       public class Student_Metadata
        {
           public string Student { get; set; }
            public List<Hw_Metadata> Hw_Metadatas { get; set; } = new List<Hw_Metadata>();
        }
        public class Hw_Metadata
        {
            
            public string Title  { get; set; }
            public int Score { get; set; }
        }
        private void settingBtn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void hwSlection_lb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (hwSlection_lb.SelectedIndex!=-1)
            {
                student.Clear();
               // student_lv.Items.Clear();

                hwDocument.Clear();
                //hwDocument_lv.Items.Clear();
                hwDocument_lv.ItemsSource = null;

                // student.Add();
                LoadStudent();
            }
        }
        private void LoadStudent()
        {
            student_lv.ItemsSource = null;

            studentHwListRead(hw[hwSlection_lb.SelectedIndex]);

            //https://www.wpf-tutorial.com/listview-control/listview-sorting/
            //sorting
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(student_lv.ItemsSource);
            view.SortDescriptions.Add(new SortDescription("id", ListSortDirection.Ascending));
        }
        
        private void studentHwListRead(string hwDirectory)
        {
            string _path = homerworker.Properties.Settings.Default.directoryPath;
            _path += "\\"+hwDirectory ;
            
            string[] studentId = Directory.GetDirectories(_path);
            if (studentId.Length != 0)
            {
                List<Student> items = new List<Student>();
                foreach (string readid in studentId)
                {
                    string hw_id = System.IO.Path.GetFileName(readid); //路徑取資料夾名
                    string id = hw_id;//hw1_109423069 取右

                    student.Add(System.IO.Path.GetFileName(readid));


                    //取HW
                    string hwSeq =(string) hwSlection_lb.SelectedItem;
                    hwSeq = hwSeq.Substring(2);

                    //Student_Metadata sm= StudentMetadata.Find(student => student.Student.Equals(id));
                    //開頭一致的傳回
                   
                   //List<Hw_Metadata> hw = sm.Hw_Metadatas.FindAll(hw_ =>
                   //hw_.Title.Split('_')
                   //.Select((string x)=>new {Titles=x[0]})
                   //.Where(x=>x.Titles==hwSeq));


                    
                    items.Add(new Student { grade = "1-1:69", id = $"{id}" });
                    
                }
                student_lv.ItemsSource = items;
            }
        }
        public class Student
        {
            //顯示於listview or lb
            public string id { get; set; }
            public string grade { get; set; }
        }
        public class HomeworkKWSK
        {

            public string Path { get; set; }
            //顯示於listview or lb
            public string homeworkName { get; set; }
            public int grade { get; set; }
        }
        

        private void student_lv_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            LoadHomework();
            
        }
        private void LoadHomework()
        {
            if (student_lv.SelectedIndex != -1)
            {
                string _path = homerworker.Properties.Settings.Default.directoryPath + "\\" + hw[hwSlection_lb.SelectedIndex] + "\\" + student[student_lv.SelectedIndex] + "\\";

                _path = DirectorySearch(_path);
                string[] document = Directory.GetFiles(_path);
                //string[] document = Directory.GetFiles(_path, "*.c");

                hwDocument.Clear();

                Student selectedItem = (Student)student_lv.SelectedItem;

                string studentID = selectedItem.id;



                List<HomeworkKWSK> hwdoc = new List<HomeworkKWSK>();

                foreach (string item in document)
                {
                    string _name = System.IO.Path.GetFileName(item);
                    Student_Metadata sm = MetadataSeek(studentID, _name);
                    int score = 0;
                    if (sm != null)
                    {
                        Hw_Metadata aa = sm.Hw_Metadatas.Find(aHomework => aHomework.Title.Equals(_name));
                        if (aa != null)
                        {
                            score = aa.Score;
                        }
                    }



                    homeworkDocName_label.Content = "檔名:";
                    score_txt.Text = "";
                    hwdoc.Add(new HomeworkKWSK { grade = score, homeworkName = $"{_name}" ,Path=item});
                }

                hwDocument_lv.ItemsSource = hwdoc;


            }
        }
        private string DirectorySearch(string directory)
        {
            string [] directories=Directory.GetDirectories(directory);
            if (directories.Length==1)
            {
                return DirectorySearch(directories[0]);
                
            }
            else if(directories.Length>1)
            {

                for (int i = 0; i < directories.Length; i++)
                {
                    string n = new DirectoryInfo(directories[i]).Name;
                    if (n== "__MACOSX")
                    {
                        directories = directories.Where(val =>new DirectoryInfo(val).Name != "__MACOSX").ToArray();
                        break;
                    }
                }

                if (directories.Length==1)
                {
                    return DirectorySearch(directories[0]);
                }
                else if(directories.Length==0)
                {
                    return directory;
                }
                
                
                System.Windows.Forms.MessageBox.Show("太多資料夾ㄌ");
                return directory;

            }
            else
            {
                return directory;

            }
            return directory;

        }

        private void hwDocument_lv_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            HomeworkKWSK selectedhomeworkKWSK = (HomeworkKWSK)hwDocument_lv.SelectedItem;

            if (selectedhomeworkKWSK!=null)
            {
                int score = selectedhomeworkKWSK.grade;
                string homeworkName = selectedhomeworkKWSK.homeworkName;

                homeworkDocName_label.Content = $"檔名:{homeworkName}";
                score_txt.Text = score.ToString();
            }
            else
            {
                homeworkDocName_label.Content = "檔名:";
                score_txt.Text = "";
            }           
        }


        private void saveBtn_Click(object sender, RoutedEventArgs e)
        {

            Student hw_ =(Student) student_lv.SelectedItem;
            HomeworkKWSK kWSK_ = (HomeworkKWSK)hwDocument_lv.SelectedItem;

            if ((hw_!=null)&&(kWSK_!=null))
            {
                string id_ = hw_.id;
                int score_ = Convert.ToInt32( score_txt.Text);
                string hwName_ = kWSK_.homeworkName;

                MetadataWrite(id_, hwName_, score_);

                LoadHomework();

            }

        }

        private void score_txt_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {

            e.Handled = !IsTextAllowed(e.Text);
        }

        private static readonly Regex _regex = new Regex("[^0-9.-]+"); //regex that matches disallowed text
        private static bool IsTextAllowed(string text)
        {
            return !_regex.IsMatch(text);
        }

        private void ImportBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!Directory.Exists("temp"))
            {
                Directory.CreateDirectory("temp");
            }
            System.IO.DirectoryInfo di = new DirectoryInfo("temp");

            foreach (FileInfo file in di.GetFiles())
            {
                file.Delete();
            }
            foreach (DirectoryInfo dir in di.GetDirectories())
            {
                dir.Delete(true);
            }

            OpenFileDialog open = new OpenFileDialog();
            
            if (open.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                ZipFile.ExtractToDirectory(open.FileName, "temp");
            }
            string [] path= Directory.GetDirectories("temp");
            if (path.Length>0)
            {
                foreach (string folder in path)
                {
                    string destPath = DirectorySearch(folder);
                    string[] zips = Directory.GetFiles(destPath, "*.zip");
                    if (zips.Length>0)
                    {
                        foreach (string zip in zips)
                        {
                            ZipFile.ExtractToDirectory(zip, DirectorySearch(folder));
                        }
                    }

                   
                }
            }
            string dire = Path.Combine(Directory.GetCurrentDirectory(), "temp");
            string proj = homerworker.Properties.Settings.Default.directoryPath;
            Process.Start("explorer.exe",dire);
            Process.Start("explorer.exe", proj);

        }

        private void hwDocument_lv_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            HomeworkKWSK kWSK=(HomeworkKWSK) hwDocument_lv.SelectedItem;
            Process.Start(kWSK.Path);
        }
    }
}
