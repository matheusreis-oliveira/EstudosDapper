using System;
using System.Collections.Generic;

namespace EstudosDapper.Models
{
    public class CareerModel
    {
        public CareerModel()
        {
            Items = new List<CareerItemModel>();
        }

        public Guid Id { get; set; }
        public string Title { get; set; }
        public IList<CareerItemModel> Items { get; set; }
    }
}