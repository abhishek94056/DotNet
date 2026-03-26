using Microsoft.EntityFrameworkCore;

namespace StudentInfoDemo.Models
{
    public class StudentInfoDbContaxt : DbContext
    {
        public StudentInfoDbContaxt(DbContextOptions option):base(option)
        {
        }

        //Create DbSet as entity class name
        DbSet<Student> Students { get; set; }   
    }
}
