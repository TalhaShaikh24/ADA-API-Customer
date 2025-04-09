using ADA.API.IRepositories;
using ADA.API.IServices;
using ADAClassLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ADA.API.Services
{
    public class CargoService : ICargoService
    {
        private readonly ICargoRepositery _CargoRepository;

        public CargoService(ICargoRepositery CargoRepository)
        {
            _CargoRepository = CargoRepository;
        }
        public Cargo Add(Cargo obj)
        {

            return _CargoRepository.Add(obj);
        }

        public List<Cargo> GetAll()
        {
            return _CargoRepository.GetAll();
        }

        public object GetCargosDropDown()
        {
            return _CargoRepository.GetCargosDropDown();
        }


        public Cargo GetCargoBtID(int id)
        {
            return _CargoRepository.GetByID(id);
        }


        public Cargo Update(Cargo obj)
        {

            return _CargoRepository.Update(obj);
        }

        public List<CargoGetFlightByDate> GetAllFlightbyDate(CargoGetFlightByDate obj)
        {
            return _CargoRepository.GetAllFlightbyDate(obj);
        }

        public List<CargoFlightDetails> GetFlightDetailsbyId(int id)
        {
            return _CargoRepository.GetFlightDetailsbyId(id);
        }

        public List<Cargo> CargoReceivedMultiUpdate(CargoMultiUpdate obj)
        {
            return _CargoRepository.CargoReceivedMultiUpdate(obj);
        }

        public List<Cargo> CargoLoadedMultiUpdate(CargoMultiUpdate obj)
        {
            return _CargoRepository.CargoLoadedMultiUpdate(obj);
        }

        public List<int> CargoReceiveDeletedMultiple(CargoMultiUpdate obj)
        {
            return _CargoRepository.CargoReceiveDeletedMultiple(obj);
        }

        public List<CargoStaff> GetAllCargoStaff()
        {
            return _CargoRepository.GetAllCargoStaff();
        }

        public CargoStaff AddStaffCargo(CargoStaff obj)
        {
            return _CargoRepository.AddStaffCargo(obj);
        }

        public CargoStaff UpdateStaffCargo(CargoStaff obj)
        {
            return _CargoRepository.UpdateStaffCargo(obj);
        }

        public CargoStaff GetStaffCargoByID(int id)
        {
            return _CargoRepository.GetStaffCargoByID(id);
        }

        public CargoExportDataFilter GetCargoExportDataDetailById(int id)
        {
            return _CargoRepository.GetCargoExportDataDetailById(id);
        }

        public CargoExportDataFilter UpdateStatusCargoExportData(CargoExportDataFilter obj)
        {
            return _CargoRepository.UpdateStatusCargoExportData(obj);
        }

        public List<Cargo> GetAllCargoStatusByBarcode(string Barcode)
        {
            return _CargoRepository.GetAllCargoStatusByBarcode(Barcode);
        }
    }
}
