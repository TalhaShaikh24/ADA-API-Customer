using ADAClassLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ADA.API.IRepositories
{
   public interface IRegisterRepository
    {
        object Add(Register obj);
        Register AddGroup(Register obj);
        Register AddOtherGroupMember(Register obj);
        Register AddOtherCorporateMember(Register obj);
        Register AddCorporate(Register obj);
        Register UpdateMembers(Register obj);
        Register update(Register obj);
        Register updateGroup(Register obj);
        Register updateCorporate(Register obj);
        Register GetDocumentByUserId(int Id);
        Register GetGroupDocumentByUserId(int Id);
        Register GetCorporateDocumentByUserId(int Id);
        List<Register> GetALLGroupMemberByGroupHeadId(int Id);
        List<Register> GetALLCorporateMemberByCorporateHeadId(int Id);
        List<Register> GetAll();
        List<Register> GetAllGroupUsersById(int Id);
        List<Register> GetAllFlightMembersByUserID(int Id);
        List<Register> DeleteGroupMember(int Id);
        List<Register> DeleteCorporateMember(int Id);
        List<Register> GetSingleMemberById(UserIdWithType obj);

        CodeVerification ForgotPassword(CodeVerification obj);
        object ResetPassword(CodeVerification obj);

        Register ActiveInactiveByUserId(int Id);
       



    }



}
