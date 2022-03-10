using System;

namespace EstudosDapper.Models
{
    public class CategoryModel
    {
        public Guid Id { get; set; }
        public string Title { get; set; } 
        public string Url { get; set; }
        public string Summary { get; set; }
        public int Order { get; set; }
        public string Description { get; set; }
        public bool Featured { get; set; }
    }
}