using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentInfoDemo.Models
{
    public class Student
    {
        [Key]
        [Column("stuid", TypeName = "int")]
        public int sid {  get; set; }
        [Column("stuname", TypeName = "varchar(50)")]
        public string name { get; set; }
        [Column("stugender", TypeName = "varchar(50)")]
        public string gender { get; set; }
        [Column("studob", TypeName = "date")]
        public DateOnly dob { get; set; }
        [Column("stuemail", TypeName = "varchar(50)")]
        public string email { get; set; }
        [Column("stuphone", TypeName = "varchar(50)")]
        public string phone { get; set; }
    }
}
