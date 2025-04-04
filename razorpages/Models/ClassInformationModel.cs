using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace YourProjectName.Models
{
    public class ClassInformationModel
    {
        private static int _idCounter = 1; // Auto-incremented ID
        public static List<ClassInformationModel> ClassList { get; set; } = new List<ClassInformationModel>();

        public int Id { get; private set; } 

        [Required]
        public string ClassName { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Student count must be at least 1")]
        public int StudentCount { get; set; }

        public string Description { get; set; }

        public ClassInformationModel(string className, int studentCount, string description)
        {
            Id = _idCounter++;
            ClassName = className;
            StudentCount = studentCount;
            Description = description;
        }

        public static void AddClass(string className, int studentCount, string description)
        {
            ClassList.Add(new ClassInformationModel(className, studentCount, description));
        }

        public static void DeleteClass(int id)
        {
            var classInfo = ClassList.FirstOrDefault(c => c.Id == id);
            if (classInfo != null)
            {
                ClassList.Remove(classInfo);
            }
        }

        public static ClassInformationModel GetClassById(int id)
        {
            return ClassList.FirstOrDefault(c => c.Id == id);
        }

        public static void UpdateClass(int id, string className, int studentCount, string description)
        {
            var classInfo = ClassList.FirstOrDefault(c => c.Id == id);
            if (classInfo != null)
            {
                classInfo.ClassName = className;
                classInfo.StudentCount = studentCount;
                classInfo.Description = description;
            }
        }
    }
}
