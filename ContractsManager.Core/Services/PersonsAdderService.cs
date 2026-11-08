using Entities;
using Microsoft.Extensions.Logging;
using RepositoryContracts;
using ServiceContracts;
using ServiceContracts.DTO;
using Services.Helpers;

namespace Services
{
  public class PersonsAdderService : IPersonsAdderService
  {
    //private field
    private readonly IPersonsRepository _personsRepository;
    private readonly ILogger<PersonsAdderService> _logger;

    //constructor
    public PersonsAdderService(IPersonsRepository personsRepository, ILogger<PersonsAdderService> logger)
    {
      _personsRepository = personsRepository;
      _logger = logger;
    }

    public async Task<PersonResponse> AddPerson(PersonAddRequest? personAddRequest)
    {

      //check if PersonAddRequest is not null
      if (personAddRequest == null)
      {
        throw new ArgumentNullException(nameof(personAddRequest));
      }

      //model Validation
      ValidationHelper.ModelValidation(personAddRequest);

      //convert personAddRequest into Person type
      Person person = personAddRequest.ToPerson();

      //generate PersonID
      person.PersonID = Guid.NewGuid();

      //add person object to person list
      await _personsRepository.AddPerson(person);

      //From Store Procedure
      //_db.sp_InsertPerson(person);

      //convert the person object into PersonResponse type
      return person.ToPersonResponse();
    }
  }
}
