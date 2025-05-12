using CarWebApi.EvDataModels.Models;
using CarWebApi.EvDataModels.DTOs;

namespace CarWebApi.Data.Repositories
{
    public interface ICarRepository
    {
        Task<Make> GetMakeByIdAsync(int id);
        Task<IdList> GetMakeIdListAsync();
        Task<List<Make>> GetAllMakesAsync();
        Task<bool> CreateMakeAsync(Make make);
        Task<bool> UpdateMakeAsync(Make make);
        Task<bool> DeleteMakeAsync(int id);

        Task<Model> GetModelByIdAsync(int id);
        Task<IdList> GetModelIdListAsync();
        Task<List<Model>> GetAllModelsAsync();
        Task<List<Model>> GetModelsByMakeIdAsync(int make_id);
        Task<bool> CreateModelAsync(Model model);
        Task<bool> UpdateModelAsync(Model model);
        Task<bool> DeleteModelAsync(int id);


        Task<Car> GetCarByIdAsync(int id);
        Task<IdList> GetCarIdListAsync();
        Task<bool> CreateCarAsync(Car car);
        Task<bool> UpdateCarAsync(Car car);
        Task<bool> DeleteCarAsync(int id);
    }
}
