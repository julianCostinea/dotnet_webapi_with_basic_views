using System.Text.Encodings.Web;
using dotnet_rpg.Data;
using dotnet_rpg.Dtos.Character;
using dotnet_rpg.Models;
using dotnet_rpg.Services.CharacterService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace dotnet_rpg.Controllers
{
    public class HelloWorldController : Controller
    {
        private readonly DataContext _context;
        public HelloWorldController(DataContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Welcome(string name, int numTimes = 1)
        {
            ViewData["Message"] = "Hello " + name;
            ViewData["NumTimes"] = numTimes;
            return View();
        }
        
        public async Task<IActionResult> Character(int id = 1)
        {
            ServiceResponse<GetCharacterDto> characterResult = null;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:7186/api/");
                var response =  await client.GetAsync("character/" + id);
                characterResult = JsonConvert.DeserializeObject<ServiceResponse<GetCharacterDto>>(response.Content.ReadAsStringAsync().Result);
                if (characterResult.Data == null)
                {
                    characterResult = new ServiceResponse<GetCharacterDto>();
                    characterResult.Success = false;
                    characterResult.Message = "Cannot find character with id " + id;
                }
            }

            // var character =  _context.Characters.FirstOrDefaultAsync(c => c.Id == id);

            // if (character == null)
            // {
            //     return NotFound();
            // }
            
            return View(characterResult);
        }
    }
}