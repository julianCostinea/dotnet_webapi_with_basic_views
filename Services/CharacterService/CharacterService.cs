using System.Security.Claims;
using AutoMapper;
using dotnet_rpg.Data;
using dotnet_rpg.Dtos.Character;
using dotnet_rpg.Models;
using Microsoft.EntityFrameworkCore;

namespace dotnet_rpg.Services.CharacterService;

public class CharacterService : ICharacterService
{
    // private static List<Character> characters = new List<Character>
    // {
    //     new Character(),
    //     new Character { Id = 1, Name = "Sam" }
    // };

    private readonly IMapper _mapper;
    private readonly DataContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CharacterService(IMapper mapper, DataContext context, IHttpContextAccessor httpContextAccessor)
    {
        _mapper = mapper;
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }

    private int GetUserId() =>
        int.Parse(_httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));

    public async Task<ServiceResponse<List<GetCharacterDto>>> GetAllCharacters()
    {
        ServiceResponse<List<GetCharacterDto>> serviceResponse = new ServiceResponse<List<GetCharacterDto>>();
        List<Character> dbCharacters = await _context.Characters.Where(c => c.User.Id == GetUserId()).ToListAsync();
        serviceResponse.Data = (dbCharacters.Select(c => _mapper.Map<GetCharacterDto>(c))).ToList();
        return serviceResponse;
    }
    // {   OLD WAY WITHOUT USER ID
    //     var response = new ServiceResponse<List<GetCharacterDto>>();
    //     var dbCharacters = await _context.Characters.ToListAsync();
    //     response.Data = (dbCharacters.Select(c => _mapper.Map<GetCharacterDto>(c))).ToList();
    //     return response;
    //     // return new ServiceResponse<List<GetCharacterDto>>
    //     // {
    //     //     Data = characters.Select(c => _mapper.Map<GetCharacterDto>(c)).ToList()
    //     // };
    // }

    public async Task<ServiceResponse<GetCharacterDto>> GetCharacterById(int id)
    {
        var serviceResponse = new ServiceResponse<GetCharacterDto>();
        // var dbCharacter = await _context.Characters.FirstOrDefaultAsync(c => c.Id == id && c.User.Id == GetUserId());
        var dbCharacter = await _context.Characters.FirstOrDefaultAsync(c => c.Id == id);
        // var character = characters.FirstOrDefault(c => c.Id == id);
        serviceResponse.Data = _mapper.Map<GetCharacterDto>(dbCharacter);
        return serviceResponse;
    }

    public async Task<ServiceResponse<List<GetCharacterDto>>> AddCharacter(AddCharacterDto newCharacter)
    {
        var serviceResponse = new ServiceResponse<List<GetCharacterDto>>();
        var character = _mapper.Map<Character>(newCharacter);
        character.User = await _context.Users.FirstOrDefaultAsync(u => u.Id == GetUserId());
        // character.Id = characters.Max(c => c.Id) + 1;
        //Add doesnt need to be async
        _context.Characters.Add(character);
        await _context.SaveChangesAsync();
        serviceResponse.Data = await _context.Characters
            .Where(c => c.User.Id == GetUserId())
            .Select(c => _mapper.Map<GetCharacterDto>(c)).ToListAsync();
        // serviceResponse.Data = characters.Select(c => _mapper.Map<GetCharacterDto>(c)).ToList();
        return serviceResponse;
    }

    public async Task<ServiceResponse<GetCharacterDto>> UpdateCharacter(UpdateCharacterDto updatedCharacter)
    {
        ServiceResponse<GetCharacterDto> response = new ServiceResponse<GetCharacterDto>();
        try
        {
            // Character character = characters.FirstOrDefault(c => c.Id == updatedCharacter.Id);
            var character = await _context.Characters.Include(c => c.User)
                .FirstOrDefaultAsync(c => c.Id == updatedCharacter.Id);

            if (character.User.Id == GetUserId())
            {
                _mapper.Map(updatedCharacter, character);
                // character.Name = updatedCharacter.Name;
                // character.Class = updatedCharacter.Class;
                // character.Defense = updatedCharacter.Defense;
                // character.HitPoints = updatedCharacter.HitPoints;
                // character.Intelligence = updatedCharacter.Intelligence;
                // character.Strength = updatedCharacter.Strength;
                await _context.SaveChangesAsync();
                response.Data = _mapper.Map<GetCharacterDto>(character);
            }
            else
            {
                response.Success = false;
                response.Message = "Character not found.";
            }
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = ex.Message;
        }

        return response;
    }

    public async Task<ServiceResponse<List<GetCharacterDto>>> DeleteCharacter(int id)
    {
        ServiceResponse<List<GetCharacterDto>> response = new ServiceResponse<List<GetCharacterDto>>();

        try
        {
            Character character =
                await _context.Characters.FirstOrDefaultAsync(c => c.Id == id && c.User.Id == GetUserId());

            if (character != null)
            {
                _context.Characters.Remove(character);

                await _context.SaveChangesAsync();
                response.Data = _context.Characters.Where(c => c.User.Id == GetUserId())
                    .Select(c => _mapper.Map<GetCharacterDto>(c)).ToList();
            }
            else
            {
                response.Success = false;
                response.Message = "Character not found.";
            }
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = ex.Message;
        }

        return response;
    }

    public async Task<ServiceResponse<GetCharacterDto>> AddCharacterSkill(AddCharacterSkillDto newCharacterSkill)
    {
        var response = new ServiceResponse<GetCharacterDto>();
        try
        {
            var character = await _context.Characters
                .Include(c => c.Weapon)
                .Include(c => c.Skills)
                .FirstOrDefaultAsync(c => c.Id == newCharacterSkill.CharacterId && c.User.Id == GetUserId());
            
            if (character == null)
            {
                response.Success = false;
                response.Message = "Character not found.";
                return response;
            }
            
            var skill = await _context.Skills.FirstOrDefaultAsync(s => s.Id == newCharacterSkill.SkillId);
            if (skill == null)
            {
                response.Success = false;
                response.Message = "Skill not found.";
                return response;
            }
            
            character.Skills.Add(skill);
            await _context.SaveChangesAsync();
            response.Data = _mapper.Map<GetCharacterDto>(character);
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = ex.Message;
        }

        return response;
    }
}