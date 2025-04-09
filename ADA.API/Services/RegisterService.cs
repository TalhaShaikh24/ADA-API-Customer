using ADA.API.IRepositories;
using ADA.API.IServices;
using ADAClassLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ADA.API.Services
{
    public class RegisterService : IRegisterService
    {
        private readonly IRegisterRepository _repository;

        public RegisterService(IRegisterRepository repository)
        {
            _repository = repository;
        }


        public object Add(Register obj)
        {
            return _repository.Add(obj);
        } 
        

       public Register AddGroup(Register obj)
        {
            return _repository.AddGroup(obj);
        }
       public Register AddOtherGroupMember(Register obj)
       {
           return _repository.AddOtherGroupMember(obj);
       }
       public Register AddOtherCorporateMember(Register obj)
       {
           return _repository.AddOtherCorporateMember(obj);
       }
       public Register AddCorporate(Register obj)
        {
            return _repository.AddCorporate(obj);
        }

        public List<Register> GetAll()
        {
            throw new NotImplementedException();
        }

        public List<Register> GetAllGroupUsersById(int Id)
        {
            return _repository.GetAllGroupUsersById(Id);
        }
         public List<Register> GetAllFlightMembersByUserID(int Id)
        {
            return _repository.GetAllFlightMembersByUserID(Id);
        }
        
        public List<Register> GetSingleMemberById(UserIdWithType obj)
        {
            return _repository.GetSingleMemberById(obj);
        }

        public Register UpdateMembers(Register obj)
        {
            return _repository.UpdateMembers(obj);
        }

        public Register update(Register obj)
        {
            return _repository.update(obj);
        }

        public Register updateGroup(Register obj)
        {
            return _repository.updateGroup(obj);
        }

        public Register updateCorporate(Register obj)
        {
            return _repository.updateCorporate(obj);
        }

        public List<Register> DeleteGroupMember(int Id)
        {
            return _repository.DeleteGroupMember(Id);
            
        }
        public List<Register> DeleteCorporateMember(int Id)
        {
            return _repository.DeleteCorporateMember(Id);
            
        }
        public Register GetDocumentByUserId(int Id)
        {
            return _repository.GetDocumentByUserId(Id);
        }
        public Register GetGroupDocumentByUserId(int Id)
        {
            return _repository.GetGroupDocumentByUserId(Id);
        }

        public List<Register> GetALLGroupMemberByGroupHeadId(int Id)
        {
            return _repository.GetALLGroupMemberByGroupHeadId(Id);
        }
        public List<Register> GetALLCorporateMemberByCorporateHeadId(int Id)
        {
            return _repository.GetALLCorporateMemberByCorporateHeadId(Id);
        }

        public Register GetCorporateDocumentByUserId(int Id)
        {
            return _repository.GetCorporateDocumentByUserId(Id);
        }

        public CodeVerification ForgotPassword(CodeVerification obj)
        {
            return _repository.ForgotPassword(obj);
        }
        public object ResetPassword(CodeVerification obj)
        {
            return _repository.ResetPassword(obj);
        }

        public Register ActiveInactiveByUserId(int Id)
        {
            return _repository.ActiveInactiveByUserId(Id);
        }
    }

}
