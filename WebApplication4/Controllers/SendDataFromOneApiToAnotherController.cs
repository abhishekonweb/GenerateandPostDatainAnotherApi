using Bogus;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplication4.Controllers.Models;

namespace WebApplication4.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SendDataFromOneApiToAnotherController : ControllerBase
    {
        private readonly Faker _faker;
        private readonly HttpClient _httpClient;

        public SendDataFromOneApiToAnotherController(Faker faker ,HttpClient httpClient)
        {
            _faker = faker;
            _httpClient = httpClient;
        }

        [HttpGet("GenerateAppData")]
        public async Task<ActionResult> GenerateAppData()
        {
            List<Student> students = await GenerateAutoData();

            var res = "";

            foreach (var student in students)

            {
               var re = await Createstudent(student);
                res+= re;
            }
            return Ok(res);
        }

        private Task<List<Student>> GenerateAutoData()
        {
            List<Student> students = new List<Student>();

            for (int i = 0; i < 100; i++)
            {
                string name = _faker.Name.FullName();
                bool isGraduated = _faker.Random.Bool();
                string gender = _faker.PickRandom("Male", "female");
                int age = _faker.Random.Int(18, 55);
                string[] courses = GenerateCourse();

                Student student = new Student
                {
                    Name = name,
                    Age = age,
                    Courses = courses,
                    IsGraduated = isGraduated,
                    Gender = gender
                };

                students.Add(student);
            }

            return Task.FromResult(students);
        }

        private string[] GenerateCourse()
        {
            List<string> courses = new List<string>();
            int numberOfCourses = _faker.Random.Int(1, 5);
            for (int i = 0; i < numberOfCourses; i++)
            {
                string course = _faker.Lorem.Word();
                courses.Add(course);
            }

            return courses.ToArray();
        }

        [HttpPost("PostData")]
        public async Task<IActionResult> Createstudent(Student student)
        {
            HttpResponseMessage message = await _httpClient.PostAsJsonAsync("https://localhost:7140/api/Student/AddStudent", student);
            if (message.IsSuccessStatusCode)
            {
                return Ok();
            }
            return BadRequest(message);
        }
    }
}
