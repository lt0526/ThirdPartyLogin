using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoginThird.Model
{
    public class DataModel
    {
        public ObservableCollection<School> Items { set; get; }

        public DataModel()
        {
            Items = new ObservableCollection<School>();

            for (int i=1; i<=500; i++)
            {
                Items.Add(new School { Name = "Item" + i });
            }
        }

    }


    public class School
    {
        public string Name { set; get; }
    }
}
