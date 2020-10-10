using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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

            MetadataWrite("30678", "5-1", 69);
            MetadataWrite("106408525", "1-1.c", 50);
            MetadataWrite("109423069", "5-8", 444);
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
                hwDocument_lv.Items.Clear();


                studentHwListRead(hw[hwSlection_lb.SelectedIndex]);
               // student.Add();

            }
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
                    string[] hw_id = System.IO.Path.GetFileName(readid).Split('_'); //路徑取資料夾名
                    string id = hw_id[1];//hw1_109423069 取右

                    id.Split('_');
                    student.Add(System.IO.Path.GetFileName(readid));
                    
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

            //顯示於listview or lb
            public string quest { get; set; }
            public int grade { get; set; }
        }
        

        private void student_lv_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            
            if (student_lv.SelectedIndex != -1)
            {
                string _path = homerworker.Properties.Settings.Default.directoryPath + "\\"+hw[hwSlection_lb.SelectedIndex]+"\\" + student[student_lv.SelectedIndex] + "\\";

                _path= DirectorySearch(_path);
                string[] document=Directory.GetFiles(_path,"*.c");

                hwDocument.Clear();

               Student selectedItem = (Student)student_lv.SelectedItem;

                string studentID = selectedItem.id;
                


                List<HomeworkKWSK> hwdoc = new List<HomeworkKWSK>();

                foreach (string item in document)
                {
                   string _name = System.IO.Path.GetFileName(item);
                    Student_Metadata sm= MetadataSeek(studentID, _name);
                    int score = 0;
                    if (sm!=null)
                    {
                       Hw_Metadata aa= sm.Hw_Metadatas.Find(aHomework =>aHomework.Title.Equals(_name));
                        if (aa!=null)
                        {
                            score = aa.Score;
                        }
                    }



                    homeworkDocName_label.Content = "檔名:";
                    score_txt.Text = "";
                    hwdoc.Add(new HomeworkKWSK { grade =score, quest = $"{_name}" });
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
                string homeworkName = selectedhomeworkKWSK.quest;

                homeworkDocName_label.Content = $"檔名:{homeworkName}";
                score_txt.Text = score.ToString();
            }
            else
            {
                homeworkDocName_label.Content = "檔名:";
                score_txt.Text = "";
            }
           



        }
    }
}
