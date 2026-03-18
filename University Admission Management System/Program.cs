using System;
using System.Collections.Generic;
using System.IO;   //file handling
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace University_Admission_Management_System
{

    // individual subjects
    class Subject
    {
        public string code { get; set; }
        public int creditHours { get; set; }
        public string subjectType { get; set; }
        public int subjectFee { get; set; }

        public Subject(string code, int creditHours, string subjectType, int subjectFee)
        {
            this.code = code;
            this.creditHours = creditHours;
            this.subjectType = subjectType;
            this.subjectFee = subjectFee;
        }
    }

    //  Degree Programs (e.g., CS, SE)
    class DegreeProgram
    {
        public string title { get; set; }
        public int duration { get; set; }
        public int seats { get; set; }
        public List<Subject> subjects { get; set; } = new List<Subject>(); // Degree "has" many subjects

        public DegreeProgram(string title, int duration, int seats)
        {
            this.title = title;
            this.duration = duration;
            this.seats = seats;
        }

        //ensure a degree doesn't become too heavy (max 20 CH)
        public bool AddSubject(Subject s)
        {
            int totalHours = 0;
            foreach (var sub in subjects) totalHours += sub.creditHours;
            if (totalHours + s.creditHours <= 20)
            {
                subjects.Add(s);
                return true;
            }
            return false;
        }
    }

    // Students applying to the university
    class Student
    {
        public string name { get; set; }
        public int age { get; set; }
        public float fscMarks { get; set; }
        public float ecatMarks { get; set; }
        public float merit { get; set; }
        public List<DegreeProgram> preferences { get; set; }                          // Student "prefers" certain degrees
        public List<Subject> regSubjects { get; set; } = new List<Subject>();          // Subjects student actually takes
        public DegreeProgram regDegree { get; set; }                                // The degree student actually gets admitted into

        public Student(string name, int age, float fsc, float ecat, List<DegreeProgram> prefs)
        {
            this.name = name;
            this.age = age;
            this.fscMarks = fsc;
            this.ecatMarks = ecat;
            this.preferences = prefs;
            calculateMerit(); // Automatically calculate merit upon creation
        }

        public void calculateMerit()
        {
            this.merit = (((fscMarks / 1100f) * 60f) + ((ecatMarks / 400f) * 40f));
        }

        // Student registration rule: Cannot exceed 9 CH
        public bool regStudentSubject(Subject s)
        {
            int currentCH = 0;
            foreach (var sub in regSubjects) currentCH += sub.creditHours;

            // Check: 1. Admitted? 2. Subject belongs to degree? 3. Within CH limit?
            if (regDegree != null && regDegree.subjects.Contains(s) && currentCH + s.creditHours <= 9)
            {
                regSubjects.Add(s);
                return true;
            }
            return false;
        }

        public int calculateFee()
        {
            int total = 0;
            foreach (var s in regSubjects) total += s.subjectFee;
            return total;
        }
    }

    class Program
    {
        // Static lists act as our "Virtual Database" while the program runs
        static List<Student> studentList = new List<Student>();
        static List<DegreeProgram> programList = new List<DegreeProgram>();
        static int boxWidth = 50;

        // Writing data to a local text file so it survives a restart
        static void SaveStudentsToFile()
        {
            string path = "students.txt";
            using (StreamWriter writer = new StreamWriter(path))
            {
                foreach (Student s in studentList)
                {   //shortcut for if else    if condition ? value if true : value if false
                    string regTitle = s.regDegree != null ? s.regDegree.title : "None";//ternary operator to handle null cases
                    writer.WriteLine($"{s.name},{s.fscMarks},{s.ecatMarks},{s.merit},{regTitle}");
                }
            }
        }

        // Reading the text file and converting lines back into Objects
        static void LoadStudentsFromFile()
        {
            string path = "students.txt";
            if (File.Exists(path))
            {
                string[] lines = File.ReadAllLines(path); // putting each line of the file into an array element
                foreach (string line in lines)
                {
                    string[] parts = line.Split(',');//splitting each line by comma to retrieve individual properties
                    if (parts.Length < 4) continue;

                    string name = parts[0];
                    float fsc = float.Parse(parts[1]);   // Convert string fsc back to float
                    float ecat = float.Parse(parts[2]);

                    Student s = new Student(name, 18, fsc, ecat, new List<DegreeProgram>());//calling cons to ret data and cre stu obj

                    // Link the student back to their admitted degree by name
                    if (parts.Length > 4 && parts[4] != "None") //count data found after split & deg title not null
                    {
                        s.regDegree = programList.Find(p => p.title == parts[4]);//find deg object by title and link to student
                    }
                    studentList.Add(s);
                }
            }
        }

        static void Main(string[] args)
        {
            LoadStudentsFromFile(); // Retrieve data on startup

            int option = 0;
            while (option != 8) // Main Control Loop
            {
                Console.Clear();
                PrintHeader();
                PrintMenu();
                DrawCenteredText("Select Option: ");
                if (!int.TryParse(Console.ReadLine(), out option)) continue; // Basic Error Handling

                switch (option)
                {
                    case 1: AddStudentUI(); break;
                    case 2: AddDegreeUI(); break;
                    case 3: GenerateMeritUI(); break;
                    case 4: ViewRegisteredStudentsUI(); break;
                    case 5: ViewByProgramUI(); break;
                    case 6: RegisterSubjectsUI(); break;
                    case 7: CalculateFeesUI(); break;
                }
            }
        }

        // --- UI DRAWING METHODS ---

        static void PrintHeader()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            DrawCenteredLine("╔══════════════════════════════════════════════════════════════╗");
            DrawCenteredLine("║               UNIVERSITY ADMISSION SYSTEM                    ║");
            DrawCenteredLine("╚══════════════════════════════════════════════════════════════╝");
            Console.ResetColor();
            Console.WriteLine();
        }

        static void PrintMenu()
        {
            DrawCenteredLine("  1. Add Student                                               ");
            DrawCenteredLine("  2. Add Degree Program                                        ");
            DrawCenteredLine("  3. Generate Merit List                                       ");
            DrawCenteredLine("  4. View Registered Students                                  ");
            DrawCenteredLine("  5. View Students of a Specific Program                       ");
            DrawCenteredLine("  6. Register Subjects                                         ");
            DrawCenteredLine("  7. Calculate Fees                                            ");
            DrawCenteredLine("  8. Exit                                                      ");
            DrawCenteredLine("---------------------------------------------------------------");
        }

        // Logic to keep the UI clean and centered regardless of window size
        static void DrawCenteredLine(string text)
        {
            int margin = (Console.WindowWidth - boxWidth) / 2;
            Console.WriteLine(new string(' ', Math.Max(0, margin)) + text);// Ensure margin(spaces) is not negative 
        }

        static void DrawCenteredText(string text)
        {
            int margin = (Console.WindowWidth - boxWidth) / 2;
            Console.Write(new string(' ', Math.Max(0, margin)) + text);// Write without newline for input prompts
        }

        static void PageEnd()
        {
            Console.WriteLine();
            DrawCenteredLine("----------------------------------------------------------------");
            DrawCenteredText("Press any key to return to Menu...");
            Console.ReadKey();
        }

        // --- CORE APPLICATION LOGIC ---

        static void AddDegreeUI()
        {
            Console.Clear();
            PrintHeader();
            DrawCenteredLine(">>> ADD NEW DEGREE PROGRAM <<<");

            DrawCenteredText("Enter Title: "); string t = Console.ReadLine();//putting directly into var
            DrawCenteredText("Enter Duration (Years): "); int dur = int.Parse(Console.ReadLine());
            DrawCenteredText("Enter Seats: "); int s = int.Parse(Console.ReadLine());

            DegreeProgram dp = new DegreeProgram(t, dur, s);

            DrawCenteredText("How many subjects? ");
            int count = int.Parse(Console.ReadLine());

            for (int i = 0; i < count; i++)
            {
                DrawCenteredText($"Subject {i + 1} Code: "); string c = Console.ReadLine();//str interpolation
                DrawCenteredText($"Subject {i + 1} CH: "); int ch = int.Parse(Console.ReadLine());// as i start from 0
                DrawCenteredText($"Subject {i + 1} Fee: "); int f = int.Parse(Console.ReadLine());// so i+1 will show 1,2,3...
                                                                                                  // if cant add ,show error
                if (!dp.AddSubject(new Subject(c, ch, "Core", f)))//core as default to fill subject type
                {
                    DrawCenteredLine("!! Limit Exceeded (20CH) !!");
                    i--; // Decrement loop counter to retry the same subject entry
                }
            }
            programList.Add(dp);
            PageEnd();
        }

        static void AddStudentUI()
        {
            Console.Clear();
            PrintHeader();     //checking if any degree programs exist before allowing student creation
            if (programList.Count == 0) { DrawCenteredLine("No programs available."); PageEnd(); return; }

            DrawCenteredText("Name: "); string n = Console.ReadLine();
            DrawCenteredText("FSC Marks: "); float fsc = float.Parse(Console.ReadLine());
            DrawCenteredText("ECAT Marks: "); float ecat = float.Parse(Console.ReadLine());

            DrawCenteredLine("-- Preferences --");// list all programs with index for user to choose from
            for (int i = 0; i < programList.Count; i++) DrawCenteredLine($"{i}. {programList[i].title}");

            List<DegreeProgram> prefs = new List<DegreeProgram>();
            DrawCenteredText("How many preferences? "); int count = int.Parse(Console.ReadLine());
            for (int i = 0; i < count; i++)
            {
                DrawCenteredText("Enter index: "); int idx = int.Parse(Console.ReadLine());
                if (idx >= 0 && idx < programList.Count) prefs.Add(programList[idx]);//Validate index and add to preferences
            }

            studentList.Add(new Student(n, 18, fsc, ecat, prefs));
            SaveStudentsToFile(); // Save immediately after adding
            PageEnd();
        }

        // THE CORE ALGORITHM: Sorts students by merit and assigns seats
        static void GenerateMeritUI()
        {    //var keyword allows compiler to infer type based on the right-hand side expression automatically,no need to write List<Student> again
            Console.Clear();
            PrintHeader();
            // LINQ(language integrated query) for sorting: Sort students descending by merit
            var sorted = studentList.OrderByDescending(st => st.merit).ToList();//putting stu with highest merit at top of list
            foreach (var student in sorted) //iterating through new sorted list to assign seats based on preferences
            {
                foreach (var pref in student.preferences) //greedy algorithm:making best choice for each student without looking ahead to future students
                {
                    // If seat available and student not already admitted elsewhere
                    if (pref.seats > 0 && student.regDegree == null)
                    {
                        student.regDegree = pref;  //pref is ref to degree program object, so this links student to that degree
                        pref.seats--; // Consume a seat
                        DrawCenteredLine($"{student.name} admitted to {pref.title}");
                        break; // Stop checking other preferences for this student
                    }
                }
            }
            SaveStudentsToFile(); // Save the new admission statuses
            PageEnd();
        }

        static void ViewRegisteredStudentsUI()
        {
            Console.Clear();
            PrintHeader();
            // Formatting output into columns  // 0 means name , -15 means 15 characters wide & left align, and so on
            DrawCenteredLine(string.Format("{0,-15} {1,-10} {2,-15}", "Name", "Merit", "Degree"));
            foreach (var s in studentList)
            {
                string d = s.regDegree != null ? s.regDegree.title : "Pending";// Handle null cases for students not admitted yet
                DrawCenteredLine(string.Format("{0,-15} {1,-10:F2} {2,-15}", s.name, s.merit, d));
            }        //:F2 formats the merit score to 2 decimal places 
            PageEnd();
        }

        static void ViewByProgramUI()
        {
            Console.Clear();
            PrintHeader();
            DrawCenteredText("Program Title: "); string t = Console.ReadLine();
            foreach (var s in studentList)
            {
                if (s.regDegree != null && s.regDegree.title == t)// Check if student is admitted and matches the requested program
                    DrawCenteredLine("- " + s.name); //list all students admitted to that program
            }
            PageEnd();
        }

        static void RegisterSubjectsUI()
        {
            Console.Clear();
            PrintHeader();
            DrawCenteredText("Student Name: ");
            string n = Console.ReadLine();

            Student s = studentList.Find(st => st.name == n);

            if (s != null && s.regDegree != null)
            {
                // Display available subjects
                for (int i = 0; i < s.regDegree.subjects.Count; i++)
                    DrawCenteredLine($"{i}. {s.regDegree.subjects[i].code}");

                // THE FIX: Ask for quantity first
                DrawCenteredText("How many subjects do you want to add? ");
                int count = int.Parse(Console.ReadLine());

                // LOOP: This allows multiple registrations in one session
                for (int i = 0; i < count; i++)
                {
                    DrawCenteredText($"Enter Index for subject {i + 1}: ");
                    int idx = int.Parse(Console.ReadLine());

                    if (idx >= 0 && idx < s.regDegree.subjects.Count)
                    {
                        // We call the object's method. It returns true/false based on CH limits.
                        if (s.regStudentSubject(s.regDegree.subjects[idx]))
                        {
                            DrawCenteredLine($">> {s.regDegree.subjects[idx].code} added.");
                        }
                        else
                        {
                            DrawCenteredLine(">> Failed. Check if already added or 9CH limit reached.");
                        }
                    }
                }
            }
            else
            {
                DrawCenteredLine("Student not found or not admitted.");
            }
            PageEnd();
        }

        static void CalculateFeesUI()
        {
            Console.Clear();
            PrintHeader();
            foreach (var s in studentList)
            {
                if (s.regDegree != null)
                    DrawCenteredLine($"{s.name}: Rs. {s.calculateFee()}");
            }
            PageEnd();
        }
    }
}