University Admission Management System (UAMS)
🏛 Project Overview
UAMS is a sophisticated console-based application engineered in C# to automate the complexities of university administration. The system manages the lifecycle of an applicant—from initial preference submission and automated merit calculation to seat allocation and subject registration.

🚀 Key Functionalities
Intelligent Seat Allocation: Uses a greedy algorithm to assign students to degree programs based on their merit rank and prioritized preferences.
Subject Management: Enforces academic constraints, such as the 20 Credit Hour (CH) limit for degree structures and 9 CH limit for individual student semester loads.
Financial Module: Dynamic fee calculation based on registered subjects and their respective credit-hour costs.
Persistent Storage: Implements custom File I/O logic to ensure data remains consistent across application restarts.

🛠 Technical Architecture (OOP Focus)
This project serves as a practical implementation of core Object-Oriented principles:PrincipleImplementation in UAMS Encapsulation Logic for merit calculation and CH validation is contained within the Student and DegreeProgram classes, shielding internal data from external interference.
CompositionModels real-world relationships: A Student "has-a" DegreeProgram, and a DegreeProgram "has-a" collection of Subject objects.AbstractionComplex processes (like the merit-sorting algorithm) are abstracted away into simple method calls in the UI layer.Data IntegrityUses conditional logic to prevent invalid states, such as registering for a subject not offered by the student's assigned degree.

💻 Installation & UsageClone the Repository:Bashgit clone https://github.com/YourUsername/UAMS-CSharp.git
Open in IDE: Load the .sln file in Visual Studio or JetBrains Rider.Run: Press F5 to compile and execute the console interface.Data Persistence: The system will automatically create a students.txt file in the build directory to store records.

📈 Future RoadmapTransitioning from public fields to C# Auto-Implemented Properties for better data hiding.Implementing Inheritance for different types of students (e.g., Undergraduate vs. Graduate).Integrating a SQL database for more robust data management.
