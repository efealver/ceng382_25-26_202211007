using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace razorpages.Models
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

        private static bool _dataGenerated = false;

        public static void GenerateFakeData(int count = 100)
        {
            if (_dataGenerated || ClassList.Count >= count)
                return;

            var rand = new Random();

            for (int i = 1; i <= count; i++)
            {
                AddClass(
                    $"Class {i}",
                    rand.Next(10, 50),
                    $"This is a description for Class {i}."
                );
            }

            _dataGenerated = true;
        }
            
    }
}
