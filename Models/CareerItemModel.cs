using System;

namespace EstudosDapper.Models
{
    public class CareerItemModel
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public CourseModel Course { get; set; }
    }
}