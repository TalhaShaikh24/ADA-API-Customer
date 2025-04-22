using ADA.API.DBManager;
using ADA.API.IRepositories;
using ADAClassLibrary;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace ADA.API.Repositories
{
    public class RegisterRepository : IRegisterRepository
    {
        private readonly IDapper _dapper;
        private readonly TextInfo myTI;

        public RegisterRepository(IDapper dapper)
        {
            myTI = new CultureInfo("en-US", false).TextInfo;
            _dapper = dapper;
        }
        public object Add(Register obj)
        {


            DynamicParameters Checkparameters = new DynamicParameters();

            //for (int i = 0; i < obj.Groups.Count(); i++)
            //{
                Checkparameters.Add("@Language", obj.Language, DbType.String, ParameterDirection.Input);
                Checkparameters.Add("@Username", obj.Username, DbType.String, ParameterDirection.Input);
                Checkparameters.Add("@Email", string.IsNullOrEmpty(obj.Email)
                    ? obj.Mobile + "@dummy.ada.ae"
                    : obj.Email, DbType.String, ParameterDirection.Input);
                Checkparameters.Add("@Documents", obj.Documents, DbType.String, ParameterDirection.Input);
                Checkparameters.Add("@Mobile", obj.Mobile, DbType.String, ParameterDirection.Input);
                Checkparameters.Add("@Birthdate", obj.Birthdate, DbType.String, ParameterDirection.Input);
                var userAvailable = _dapper.Get<Register>(@"[dbo].[CheckUserAvailability]", Checkparameters);
            //}



            var Dtype = obj.DocumentType[0].Split(',').ToList();
            string Name = string.Join(",", Dtype);
            var table = new DataTable();
            table.Columns.Add("RegisterFkId", typeof(int));
            table.Columns.Add("FilePathName", typeof(string));
            table.Columns.Add("DocumentType", typeof(string));


            for (int i = 0; i < obj.FileName.Count(); i++)
            {
                string DName = obj.DocumentType.ToList()[0].Split(',')[i];

                var row = table.NewRow();
                row["RegisterFkId"] = 0;
                row["FilePathName"] = obj.FileName[i];
                row["DocumentType"] = Convert.ToString(DName);
                table.Rows.Add(row);
            }



            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@tvDocumentType", table.AsTableValuedParameter("dbo.tvDocumentType"));
            parameters.Add("@Honorifics", obj.Honorifics == null ? "" : obj.Honorifics, DbType.String, ParameterDirection.Input);


            parameters.Add("@Language", (obj.Language == "" || obj.Language == null ? "En" : obj.Language), DbType.String, ParameterDirection.Input);
            parameters.Add("@Name", obj.Name == null ? "" : obj.Name, DbType.String, ParameterDirection.Input);
            parameters.Add("@Username", obj.Username == null ? "" : obj.Username, DbType.String, ParameterDirection.Input);
            parameters.Add("@Password", obj.Password == null ? "" : obj.Password, DbType.String, ParameterDirection.Input);
            parameters.Add("@Email", string.IsNullOrEmpty(obj.Email)
                    ? obj.Mobile + "@dummy.ada.ae"
                    : obj.Email, DbType.String, ParameterDirection.Input);
            parameters.Add("@Nationality", obj.Nationality == null ? "" : obj.Nationality, DbType.String, ParameterDirection.Input);
            parameters.Add("@Birthdate", obj.Birthdate, DbType.DateTime, ParameterDirection.Input);
            parameters.Add("@Documents", obj.Documents == null ? "" : obj.Documents, DbType.String, ParameterDirection.Input);
            //parameters.Add("@DocumentType", "" == null ? "" : "", DbType.String, ParameterDirection.Input);
            parameters.Add("@Mobile", obj.Mobile == null ? "" : obj.Mobile, DbType.String, ParameterDirection.Input);
            parameters.Add("@CreatedOn", obj.CreatedOn, DbType.DateTime, ParameterDirection.Input);
            parameters.Add("@Active", obj.Active, DbType.Boolean, ParameterDirection.Input);
            parameters.Add("@IsDelmaIsland", obj.IsDelmaIsland, DbType.Boolean, ParameterDirection.Input);
            parameters.Add("@IsUAEId", obj.IsUAEId, DbType.Boolean, ParameterDirection.Input);


            var data = _dapper.Insert<Register>(@"[dbo].[usp_AddRegister]", parameters);

            return data;
        }

        //public Register AddGroup(Register obj)
        //{

        //    DynamicParameters parameters = new DynamicParameters();

        //    var table = new DataTable();
        //    table.Columns.Add("Email", typeof(string));
        //    table.Columns.Add("Mobile", typeof(string));
        //    table.Columns.Add("Passport", typeof(string));


        //    foreach (var item in obj.Groups)
        //    {
        //        var row = table.NewRow();


        //        row["Email"] = item.Email;
        //        row["Mobile"] = item.Mobile;
        //        row["Passport"] = item.Documents;
        //        table.Rows.Add(row);

        //    }

        //    parameters.Add("@Language", obj.Language, DbType.String, ParameterDirection.Input);
        //    parameters.Add("@tvGroupCheckAvailability", table.AsTableValuedParameter("dbo.tvGroupCheckAvailability"));
        //    parameters.Add("@Username", obj.Username == null ? "" : obj.Username, DbType.String, ParameterDirection.Input);
        //    parameters.Add("@Password", obj.Password == null ? "" : obj.Password, DbType.String, ParameterDirection.Input);
        //    parameters.Add("@IsDelmaIsland", obj.IsDelmaIsland, DbType.Boolean, ParameterDirection.Input);
        //    parameters.Add("@IsUAEId", obj.IsUAEId, DbType.Boolean, ParameterDirection.Input);
        //    parameters.Add("@Active", obj.Active, DbType.Boolean, ParameterDirection.Input);

        //    parameters.Add("@CreatedOn", obj.CreatedOn, DbType.DateTime, ParameterDirection.Input);
        //    var data = _dapper.Insert<Register>(@"[dbo].[usp_AddGroupRegister]", parameters);

        //    if (data != null)
        //    {
        //        DynamicParameters parameters2 = new DynamicParameters();

        //        for (int i = 0; i < obj.Groups.Count(); i++)
        //        {


        //            parameters2.Add("@Language", obj.Groups[i].Language, DbType.String, ParameterDirection.Input);
        //            parameters2.Add("@tvGroupCheckAvailability", table.AsTableValuedParameter("dbo.tvGroupCheckAvailability"));
        //            parameters2.Add("@Honorifics", obj.Groups[i].Honorifics, DbType.String, ParameterDirection.Input);
        //            parameters2.Add("@Name",  obj.Groups[i].Name, DbType.String, ParameterDirection.Input);
        //            parameters2.Add("@Email", obj.Groups[i].Email, DbType.String, ParameterDirection.Input);
        //            parameters2.Add("@Nationality", obj.Groups[i].Nationality, DbType.String, ParameterDirection.Input);
        //            parameters2.Add("@Birthdate", obj.Groups[i].Birthdate, DbType.DateTime, ParameterDirection.Input);
        //            parameters2.Add("@Documents", obj.Groups[i].Documents, DbType.String, ParameterDirection.Input);
        //            parameters2.Add("@Mobile", obj.Groups[i].Mobile, DbType.String, ParameterDirection.Input);
        //            parameters2.Add("@IsDelmaIsland", obj.Groups[i].IsDelmaIsland, DbType.Boolean, ParameterDirection.Input);
        //            parameters2.Add("@IsUAEId",  obj.Groups[i].IsUAEId, DbType.Boolean, ParameterDirection.Input);
        //            parameters2.Add("@CreatedOn", obj.CreatedOn, DbType.DateTime, ParameterDirection.Input);
        //            parameters2.Add("@Active", true, DbType.Boolean, ParameterDirection.Input);
        //            parameters2.Add("@IsHead", obj.Groups[i].IsHead, DbType.Boolean, ParameterDirection.Input);
        //            parameters2.Add("@FKRegisterId",data.Id, DbType.Int32, ParameterDirection.Input);


        //            var data2 = _dapper.Insert<RegisterGroup>(@"[dbo].[usp_AddGroupInnerRegister]", parameters2);


        //            if (data2!=null)
        //            {
        //                DynamicParameters parameters3 = new DynamicParameters();



        //                for (int j = 0; j < obj.Groups[i].DocumentType.ToList().Count(); j++)

        //                {
        //                    string DName = obj.Groups[i].DocumentType[j];

        //                    parameters3.Add("@RegisterFkId", data.Id, DbType.Int32, ParameterDirection.Input);
        //                    parameters3.Add("@RegisterGroupFkId", data2.Id, DbType.Int32, ParameterDirection.Input);
        //                    parameters3.Add("@FilePathName", obj.Groups[i].FileName[j], DbType.String, ParameterDirection.Input);
        //                    parameters3.Add("@DocumentType", DName, DbType.String, ParameterDirection.Input);
        //                    _dapper.Insert(@"[dbo].[usp_AddGroupRegisterDocument]", parameters3);


        //                }


        //            }


        //        }



        //    }



        //    return data;
        //}

        public Register AddGroup(Register obj)
        {

            DynamicParameters Checkparameters = new DynamicParameters();

            for (int i = 0; i < obj.Groups.Count(); i++)
            {
                Checkparameters.Add("@Language", obj.Groups[i].Language, DbType.String, ParameterDirection.Input);
                Checkparameters.Add("@Username", obj.Username, DbType.String, ParameterDirection.Input);
                Checkparameters.Add("@Email", string.IsNullOrEmpty(obj.Groups[i].Email)
                    ? obj.Groups[i].Mobile + "@dummy.ada.ae"
                    : obj.Groups[i].Email, DbType.String, ParameterDirection.Input);
                Checkparameters.Add("@Documents", obj.Groups[i].Documents, DbType.String, ParameterDirection.Input);
                Checkparameters.Add("@Mobile", obj.Groups[i].Mobile, DbType.String, ParameterDirection.Input);
                Checkparameters.Add("@Birthdate", obj.Groups[i].Birthdate, DbType.String, ParameterDirection.Input);
                var userAvailable = _dapper.Get<RegisterGroup>(@"[dbo].[CheckUserAvailability]", Checkparameters);
            }
          

            DynamicParameters parameters = new DynamicParameters();
          
            parameters.Add("@Language", obj.Language, DbType.String, ParameterDirection.Input);
            parameters.Add("@Username", obj.Username == null ? "" : obj.Username, DbType.String, ParameterDirection.Input);
            parameters.Add("@Password", obj.Password == null ? "" : obj.Password, DbType.String, ParameterDirection.Input);
            parameters.Add("@IsDelmaIsland", obj.IsDelmaIsland, DbType.Boolean, ParameterDirection.Input);
            parameters.Add("@IsUAEId", obj.IsUAEId, DbType.Boolean, ParameterDirection.Input);
            parameters.Add("@CreatedOn", obj.CreatedOn, DbType.DateTime, ParameterDirection.Input);
            parameters.Add("@Active", obj.Active, DbType.Boolean, ParameterDirection.Input);

            var data = _dapper.Insert<Register>(@"[dbo].[usp_AddGroupRegister]", parameters);

            if (data != null)
            {
                DynamicParameters parameters2 = new DynamicParameters();

                for (int i = 0; i < obj.Groups.Count(); i++)
                {

                    parameters2.Add("@Language", obj.Groups[i].Language, DbType.String, ParameterDirection.Input);
                    parameters2.Add("@Honorifics", obj.Groups[i].Honorifics, DbType.String, ParameterDirection.Input);
                    parameters2.Add("@Name", obj.Groups[i].Name, DbType.String, ParameterDirection.Input);

                    parameters2.Add("@Email", string.IsNullOrEmpty(obj.Groups[i].Email)
                    ? obj.Groups[i].Mobile + "@dummy.ada.ae"
                    : obj.Groups[i].Email, DbType.String, ParameterDirection.Input);

                    parameters2.Add("@Nationality", obj.Groups[i].Nationality, DbType.String, ParameterDirection.Input);
                    parameters2.Add("@Birthdate", obj.Groups[i].Birthdate, DbType.DateTime, ParameterDirection.Input);
                    parameters2.Add("@Documents", obj.Groups[i].Documents, DbType.String, ParameterDirection.Input);
                    parameters2.Add("@Mobile", obj.Groups[i].Mobile, DbType.String, ParameterDirection.Input);
                    parameters2.Add("@IsDelmaIsland", obj.Groups[i].IsDelmaIsland, DbType.Boolean, ParameterDirection.Input);
                    parameters2.Add("@IsUAEId", obj.Groups[i].IsUAEId, DbType.Boolean, ParameterDirection.Input);
                    parameters2.Add("@CreatedOn", obj.CreatedOn, DbType.DateTime, ParameterDirection.Input);
                    parameters2.Add("@Active", true, DbType.Boolean, ParameterDirection.Input);
                    parameters2.Add("@IsHead", obj.Groups[i].IsHead, DbType.Boolean, ParameterDirection.Input);
                    parameters2.Add("@FKRegisterId", data.Id, DbType.Int32, ParameterDirection.Input);
                    //parameters2.Add("@tvGroupCheckAvailability", table.AsTableValuedParameter("dbo.tvGroupCheckAvailability"));

                    var data2 = _dapper.Insert<RegisterGroup>(@"[dbo].[usp_AddGroupInnerRegister]", parameters2);


                    if (data2 != null)
                    {
                        DynamicParameters parameters3 = new DynamicParameters();



                        for (int j = 0; j < obj.Groups[i].DocumentType.ToList().Count(); j++)
                        {
                            string DName = obj.Groups[i].DocumentType[j];

                            parameters3.Add("@RegisterFkId", data.Id, DbType.Int32, ParameterDirection.Input);
                            parameters3.Add("@RegisterGroupFkId", data2.Id, DbType.Int32, ParameterDirection.Input);
                            parameters3.Add("@FilePathName", obj.Groups[i].FileName[j], DbType.String, ParameterDirection.Input);
                            parameters3.Add("@DocumentType", DName, DbType.String, ParameterDirection.Input);
                            _dapper.Insert(@"[dbo].[usp_AddGroupRegisterDocument]", parameters3);



                        }


                    }


                }

            }

            return data;
        }
        public Register AddCorporate(Register obj)
        {



            DynamicParameters Checkparameters = new DynamicParameters();
            var table = new DataTable();
            table.Columns.Add("Email", typeof(string));
            table.Columns.Add("Mobile", typeof(string));
            table.Columns.Add("Passport", typeof(string));


            foreach (var item in obj.registerCorporate)
            {
                var row = table.NewRow();


                row["Email"] = string.IsNullOrEmpty(item.Email)
                    ? item.Mobile + "@dummy.ada.ae"
                    : item.Email;
                row["Mobile"] = item.Mobile;
                row["Passport"] = item.Documents;
                table.Rows.Add(row);

            }


            for (int i = 0; i < obj.registerCorporate.Count(); i++)
            {
                Checkparameters.Add("@Language", obj.registerCorporate[i].Language, DbType.String, ParameterDirection.Input);
                Checkparameters.Add("@Username", obj.Username, DbType.String, ParameterDirection.Input);
                Checkparameters.Add("@Email", string.IsNullOrEmpty(obj.registerCorporate[i].Email)
                    ? obj.registerCorporate[i].Mobile + "@dummy.ada.ae"
                    : obj.registerCorporate[i].Email, DbType.String, ParameterDirection.Input);
                Checkparameters.Add("@Documents", obj.registerCorporate[i].Documents, DbType.String, ParameterDirection.Input);
                Checkparameters.Add("@Mobile", obj.registerCorporate[i].Mobile, DbType.String, ParameterDirection.Input);
                Checkparameters.Add("@Birthdate", obj.registerCorporate[i].Birthdate, DbType.String, ParameterDirection.Input);
            }
            _dapper.Get<RegisterCorporate>(@"[dbo].[CheckUserAvailability]", Checkparameters);




            DynamicParameters parameters = new DynamicParameters();
            //var table = new DataTable();
            //table.Columns.Add("Email", typeof(string));
            //table.Columns.Add("Mobile", typeof(string));
            //table.Columns.Add("Passport", typeof(string));


            //foreach (var item in obj.registerCorporate)
            //{
            //    var row = table.NewRow();


            //    row["Email"] = item.Email;
            //    row["Mobile"] = item.Mobile;
            //    row["Passport"] = item.Documents;
            //    table.Rows.Add(row);

            //}
            parameters.Add("@Language", obj.Language, DbType.String, ParameterDirection.Input);
            parameters.Add("@Username", obj.Username == null ? "" : obj.Username, DbType.String, ParameterDirection.Input);
            parameters.Add("@Password", obj.Password == null ? "" : obj.Password, DbType.String, ParameterDirection.Input);
            parameters.Add("@IsDelmaIsland", obj.IsDelmaIsland, DbType.Boolean, ParameterDirection.Input);
            parameters.Add("@IsUAEId", obj.IsUAEId, DbType.Boolean, ParameterDirection.Input);
            parameters.Add("@GovEntity", obj.GovEntity, DbType.Boolean, ParameterDirection.Input);
            parameters.Add("@CreatedOn", obj.CreatedOn, DbType.DateTime, ParameterDirection.Input);
            parameters.Add("@Active", obj.Active, DbType.Boolean, ParameterDirection.Input);
            //parameters.Add("@tvGroupCheckAvailability", table.AsTableValuedParameter("dbo.tvGroupCheckAvailability"));

            var data = _dapper.Insert<Register>(@"[dbo].[usp_AddCorporateRegister]", parameters);

            if (data != null)
            {
                DynamicParameters parameters2 = new DynamicParameters();

                for (int i = 0; i < obj.registerCorporate.Count(); i++)
                {

                    parameters2.Add("@Language", obj.registerCorporate[i].Language, DbType.String, ParameterDirection.Input);
                    parameters2.Add("@Honorifics", obj.registerCorporate[i].Honorifics, DbType.String, ParameterDirection.Input);
                    parameters2.Add("@Name", obj.registerCorporate[i].Name, DbType.String, ParameterDirection.Input);
                    parameters2.Add("@Email", string.IsNullOrEmpty(obj.registerCorporate[i].Email)
                    ? obj.registerCorporate[i].Mobile + "@dummy.ada.ae"
                    : obj.registerCorporate[i].Email, DbType.String, ParameterDirection.Input);
                    parameters2.Add("@Nationality", obj.registerCorporate[i].Nationality, DbType.String, ParameterDirection.Input);
                    parameters2.Add("@Birthdate", obj.registerCorporate[i].Birthdate, DbType.DateTime, ParameterDirection.Input);
                    parameters2.Add("@Documents", obj.registerCorporate[i].Documents, DbType.String, ParameterDirection.Input);
                    parameters2.Add("@Mobile", obj.registerCorporate[i].Mobile, DbType.String, ParameterDirection.Input);
                    parameters2.Add("@IsDelmaIsland", obj.registerCorporate[i].IsDelmaIsland, DbType.Boolean, ParameterDirection.Input);
                    parameters2.Add("@IsUAEId", obj.registerCorporate[i].IsUAEId, DbType.Boolean, ParameterDirection.Input);
                    parameters2.Add("@GovEntity", obj.registerCorporate[i].GovEntity, DbType.Boolean, ParameterDirection.Input);
                    parameters2.Add("@CreatedOn", obj.CreatedOn, DbType.DateTime, ParameterDirection.Input);
                    parameters2.Add("@Active", true, DbType.Boolean, ParameterDirection.Input);
                    parameters2.Add("@IsHead", obj.registerCorporate[i].IsHead, DbType.Boolean, ParameterDirection.Input);
                    parameters2.Add("@FKRegisterId", data.Id, DbType.Int32, ParameterDirection.Input);
                    //parameters2.Add("@tvGroupCheckAvailability", table.AsTableValuedParameter("dbo.tvGroupCheckAvailability"));

                    var data2 = _dapper.Insert<RegisterCorporate>(@"[dbo].[usp_AddCorporateInnerRegister]", parameters2);


                    if (data2 != null)
                    {
                        DynamicParameters parameters3 = new DynamicParameters();



                        for (int j = 0; j < obj.registerCorporate[i].DocumentType.ToList().Count(); j++)
                        {
                            string DName = obj.registerCorporate[i].DocumentType[j];

                            parameters3.Add("@RegisterFkId", data.Id, DbType.Int32, ParameterDirection.Input);
                            parameters3.Add("@RegisterCorporateFkId", data2.Id, DbType.Int32, ParameterDirection.Input);
                            parameters3.Add("@FilePathName", obj.registerCorporate[i].FileName[j], DbType.String, ParameterDirection.Input);
                            parameters3.Add("@DocumentType", DName, DbType.String, ParameterDirection.Input);
                            _dapper.Insert(@"[dbo].[usp_AddCorporateRegisterDocument]", parameters3);



                        }


                    }


                }



            }


            return data;
        }

        public List<Register> GetAll()
        {
            throw new NotImplementedException();
        }

        public List<Register> GetAllGroupUsersById(int Id)
        {
            DynamicParameters parameters = new DynamicParameters();

            parameters.Add("@Id", Id, DbType.Int32, ParameterDirection.Input);


            var data = _dapper.GetAll<Register>(@"[dbo].[GetAllGroupUsersByID]", parameters);


            return data;
        }
        public List<Register> GetAllFlightMembersByUserID(int Id)
        {
            DynamicParameters parameters = new DynamicParameters();

            parameters.Add("@Id", Id, DbType.Int32, ParameterDirection.Input);


            var data = _dapper.GetAll<Register>(@"[dbo].[GetAllFlightMembersByUserID]", parameters);


            return data;
        }
        public List<Register> DeleteGroupMember(int Id)
        {
            DynamicParameters parameters = new DynamicParameters();

            parameters.Add("@MemberId", Id, DbType.Int32, ParameterDirection.Input);


            var data = _dapper.GetAll<Register>(@"[dbo].[DeleteGroupMember]", parameters);


            return data;
        }
        public List<Register> DeleteCorporateMember(int Id)
        {
            DynamicParameters parameters = new DynamicParameters();

            parameters.Add("@MemberId", Id, DbType.Int32, ParameterDirection.Input);


            var data = _dapper.GetAll<Register>(@"[dbo].[DeleteCorporateMember]", parameters);


            return data;
        }

        public List<Register> GetSingleMemberById(UserIdWithType obj)
        {
            DynamicParameters parameters = new DynamicParameters();

            parameters.Add("@Id", obj.Id, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@RegisterType", obj.MemberType == null ? "" : obj.MemberType, DbType.String, ParameterDirection.Input);


            var data = _dapper.GetAll<Register>(@"[dbo].[GetSingleMemberById]", parameters);


            return data;
        }

        public Register UpdateMembers(Register obj)
        {

            var Dtype = obj.DocumentType[0].Split(',').ToList();
            DynamicParameters parameters = new DynamicParameters();

            if (obj.MemberType == "Group")
            {
                var table = new DataTable();

                if (obj.FileName != null)
                {


                    table.Columns.Add("RegisterGroupFkId", typeof(int));
                    table.Columns.Add("FilePathName", typeof(string));
                    table.Columns.Add("DocumentType", typeof(string));


                    for (int i = 0; i < obj.FileName.Count(); i++)
                    {
                        string DName = obj.DocumentType.ToList()[0].Split(',')[i];

                        var row = table.NewRow();
                        row["RegisterGroupFkId"] = 0;
                        row["FilePathName"] = obj.FileName[i];
                        row["DocumentType"] = Convert.ToString(DName);
                        table.Rows.Add(row);
                    }

                    parameters.Add("@tv_RegisterGroup", table.AsTableValuedParameter("dbo.tvRegisterGroup"));

                }

                parameters.Add("@MemberType", obj.MemberType == null ? "" : obj.MemberType, DbType.String, ParameterDirection.Input);
                parameters.Add("@UserId", obj.UserId, DbType.Int32, ParameterDirection.Input);
                parameters.Add("@Id", obj.Id, DbType.Int32, ParameterDirection.Input);
                parameters.Add("@Honorifics", obj.Honorifics == null ? "" : obj.Honorifics, DbType.String, ParameterDirection.Input);
                parameters.Add("@Name", obj.Name == null ? "" : obj.Name, DbType.String, ParameterDirection.Input);
                parameters.Add("@Email", obj.Email == null ? "" : obj.Email, DbType.String, ParameterDirection.Input);
                parameters.Add("@Nationality", obj.Nationality == null ? "" : obj.Nationality, DbType.String, ParameterDirection.Input);
                parameters.Add("@Birthdate", obj.Birthdate, DbType.DateTime, ParameterDirection.Input);
                parameters.Add("@Documents", obj.Documents == null ? "" : obj.Documents, DbType.String, ParameterDirection.Input);
                parameters.Add("@Mobile", obj.Mobile == null ? "" : obj.Mobile, DbType.String, ParameterDirection.Input);
                parameters.Add("@Active", obj.Active, DbType.Boolean, ParameterDirection.Input);
                parameters.Add("@IsDelmaIsland", obj.IsDelmaIsland, DbType.Boolean, ParameterDirection.Input);
                parameters.Add("@IsUAEId", obj.IsUAEId, DbType.Boolean, ParameterDirection.Input);
                parameters.Add("@GovEntity", obj.GovEntity, DbType.Boolean, ParameterDirection.Input);
            }

            if (obj.MemberType == "Corporate")
            {
                var table = new DataTable();

                if (obj.FileName != null)
                {
                    table.Columns.Add("RegisterCorporateFkId", typeof(int));
                    table.Columns.Add("FilePathName", typeof(string));
                    table.Columns.Add("DocumentType", typeof(string));


                    for (int i = 0; i < obj.FileName.Count(); i++)
                    {
                        string DName = obj.DocumentType.ToList()[0].Split(',')[i];

                        var row = table.NewRow();
                        row["RegisterCorporateFkId"] = 0;
                        row["FilePathName"] = obj.FileName[i];
                        row["DocumentType"] = Convert.ToString(DName);
                        table.Rows.Add(row);
                    }


                    parameters.Add("@tvRegisterCorporate", table.AsTableValuedParameter("dbo.tvRegisterCorporate"));
                }
                parameters.Add("@MemberType", obj.MemberType == null ? "" : obj.MemberType, DbType.String, ParameterDirection.Input);
                parameters.Add("@UserId", obj.UserId, DbType.Int32, ParameterDirection.Input);
                parameters.Add("@Id", obj.Id, DbType.Int32, ParameterDirection.Input);
                parameters.Add("@Honorifics", obj.Honorifics == null ? "" : obj.Honorifics, DbType.String, ParameterDirection.Input);
                parameters.Add("@Name", obj.Name == null ? "" : obj.Name, DbType.String, ParameterDirection.Input);
                parameters.Add("@Email", obj.Email == null ? "" : obj.Email, DbType.String, ParameterDirection.Input);
                parameters.Add("@Nationality", obj.Nationality == null ? "" : obj.Nationality, DbType.String, ParameterDirection.Input);
                parameters.Add("@Birthdate", obj.Birthdate, DbType.DateTime, ParameterDirection.Input);
                parameters.Add("@Documents", obj.Documents == null ? "" : obj.Documents, DbType.String, ParameterDirection.Input);
                parameters.Add("@Mobile", obj.Mobile == null ? "" : obj.Mobile, DbType.String, ParameterDirection.Input);
                parameters.Add("@Active", obj.Active, DbType.Boolean, ParameterDirection.Input);
                parameters.Add("@IsDelmaIsland", obj.IsDelmaIsland, DbType.Boolean, ParameterDirection.Input);
                parameters.Add("@IsUAEId", obj.IsUAEId, DbType.Boolean, ParameterDirection.Input);
                parameters.Add("@GovEntity", obj.GovEntity, DbType.Boolean, ParameterDirection.Input);
            }

            if (obj.MemberType == "Individual")
            {
                var table = new DataTable();

                if (obj.FileName != null)
                {
                    table.Columns.Add("RegisterFkId", typeof(int));
                    table.Columns.Add("FilePathName", typeof(string));
                    table.Columns.Add("DocumentType", typeof(string));


                    for (int i = 0; i < obj.FileName.Count(); i++)
                    {
                        string DName = obj.DocumentType.ToList()[0].Split(',')[i];

                        var row = table.NewRow();
                        row["RegisterFkId"] = 0;
                        row["FilePathName"] = obj.FileName[i];
                        row["DocumentType"] = Convert.ToString(DName);
                        table.Rows.Add(row);
                    }


                    parameters.Add("@tvRegisterIndividual", table.AsTableValuedParameter("dbo.tvRegisterIndividual"));
                }
                parameters.Add("@MemberType", obj.MemberType == null ? "" : obj.MemberType, DbType.String, ParameterDirection.Input);
                parameters.Add("@UserId", obj.UserId, DbType.Int32, ParameterDirection.Input);
                parameters.Add("@Id", obj.Id, DbType.Int32, ParameterDirection.Input);
                parameters.Add("@Honorifics", obj.Honorifics == null ? "" : obj.Honorifics, DbType.String, ParameterDirection.Input);
                parameters.Add("@Name", obj.Name == null ? "" : obj.Name, DbType.String, ParameterDirection.Input);
                parameters.Add("@Username", obj.Username == null ? "" : obj.Username, DbType.String, ParameterDirection.Input);
                parameters.Add("@Email", obj.Email == null ? "" : obj.Email, DbType.String, ParameterDirection.Input);
                parameters.Add("@Nationality", obj.Nationality == null ? "" : obj.Nationality, DbType.String, ParameterDirection.Input);
                parameters.Add("@Birthdate", obj.Birthdate, DbType.DateTime, ParameterDirection.Input);
                parameters.Add("@Documents", obj.Documents == null ? "" : obj.Documents, DbType.String, ParameterDirection.Input);
                parameters.Add("@Mobile", obj.Mobile == null ? "" : obj.Mobile, DbType.String, ParameterDirection.Input);
                parameters.Add("@Active", obj.Active, DbType.Boolean, ParameterDirection.Input);
                parameters.Add("@IsDelmaIsland", obj.IsDelmaIsland, DbType.Boolean, ParameterDirection.Input);
                parameters.Add("@IsUAEId", obj.IsUAEId, DbType.Boolean, ParameterDirection.Input);
                parameters.Add("@GovEntity", obj.GovEntity, DbType.Boolean, ParameterDirection.Input);
            }

            var data = _dapper.Insert<Register>(@"[dbo].[usp_UpdateGroupMembers]", parameters);

            return data;
        }

        public Register AddOtherGroupMember(Register obj)
        {

            Register Sendobj = new Register();

            DynamicParameters Checkparameters = new DynamicParameters();
            var table = new DataTable();
            table.Columns.Add("Email", typeof(string));
            table.Columns.Add("Mobile", typeof(string));
            table.Columns.Add("Passport", typeof(string));


            foreach (var item in obj.Groups)
            {
                var row = table.NewRow();


                row["Email"] = string.IsNullOrEmpty(item.Email)
                    ? item.Mobile + "@dummy.ada.ae"
                    : item.Email;
                row["Mobile"] = item.Mobile;
                row["Passport"] = item.Documents;
                table.Rows.Add(row);

            }


            for (int i = 0; i < obj.Groups.Count(); i++)
            {
                Checkparameters.Add("@Language", obj.Groups[i].Language, DbType.String, ParameterDirection.Input);
                Checkparameters.Add("@Username", obj.Username, DbType.String, ParameterDirection.Input);
                Checkparameters.Add("@Email", obj.Groups[i].Email, DbType.String, ParameterDirection.Input);
                Checkparameters.Add("@Email", string.IsNullOrEmpty(obj.Groups[i].Email)
                    ? obj.Groups[i].Mobile + "@dummy.ada.ae"
                    : obj.Groups[i].Email, DbType.String, ParameterDirection.Input);
                Checkparameters.Add("@Documents", obj.Groups[i].Documents, DbType.String, ParameterDirection.Input);
                Checkparameters.Add("@Mobile", obj.Groups[i].Mobile, DbType.String, ParameterDirection.Input);
                Checkparameters.Add("@Birthdate", obj.Groups[i].Birthdate, DbType.String, ParameterDirection.Input);
            }
               _dapper.Get<RegisterGroup>(@"[dbo].[CheckUserAvailability]", Checkparameters);



            DynamicParameters parameters = new DynamicParameters();

            parameters.Add("@Language", obj.Groups[0].Language, DbType.String, ParameterDirection.Input);
            parameters.Add("@Honorifics", obj.Groups[0].Honorifics, DbType.String, ParameterDirection.Input);
            parameters.Add("@Name", obj.Groups[0].Name, DbType.String, ParameterDirection.Input);
            parameters.Add("@Email", obj.Groups[0].Email, DbType.String, ParameterDirection.Input);
            parameters.Add("@Email", string.IsNullOrEmpty(obj.Groups[0].Email)
                    ? obj.Groups[0].Mobile + "@dummy.ada.ae"
                    : obj.Groups[0].Email, DbType.String, ParameterDirection.Input);
            parameters.Add("@Nationality", obj.Groups[0].Nationality, DbType.String, ParameterDirection.Input);
            parameters.Add("@Birthdate", obj.Groups[0].Birthdate, DbType.DateTime, ParameterDirection.Input);
            parameters.Add("@Documents", obj.Groups[0].Documents, DbType.String, ParameterDirection.Input);
            parameters.Add("@Mobile", obj.Groups[0].Mobile, DbType.String, ParameterDirection.Input);
            parameters.Add("@IsDelmaIsland", obj.Groups[0].IsDelmaIsland, DbType.Boolean, ParameterDirection.Input);
            parameters.Add("@IsUAEId", obj.Groups[0].IsUAEId, DbType.Boolean, ParameterDirection.Input);
            parameters.Add("@CreatedOn", DateTime.Now, DbType.DateTime2, ParameterDirection.Input);
            parameters.Add("@Active", true, DbType.Boolean, ParameterDirection.Input);
            parameters.Add("@IsHead", obj.Groups[0].IsHead, DbType.Boolean, ParameterDirection.Input);
            parameters.Add("@FKRegisterId", obj.Groups[0].UserId, DbType.Int32, ParameterDirection.Input);
            //parameters.Add("@tvGroupCheckAvailability", table.AsTableValuedParameter("dbo.tvGroupCheckAvailability"));

            var data2 = _dapper.Insert<RegisterGroup>(@"[dbo].[usp_AddGroupInnerRegister]", parameters);

           //Sendobj.Groups.Add(data2);

            if (data2 != null)
            {

                DynamicParameters parameters2 = new DynamicParameters();


                for (int j = 0; j < obj.Groups[0].DocumentType.ToList().Count(); j++)

                {
                    string DName = obj.Groups[0].DocumentType[j];

                    parameters2.Add("@RegisterFkId", obj.Groups[0].UserId, DbType.Int32, ParameterDirection.Input);
                    parameters2.Add("@RegisterGroupFkId", data2.Id, DbType.Int32, ParameterDirection.Input);
                    parameters2.Add("@FilePathName", obj.Groups[0].FileName[j], DbType.String, ParameterDirection.Input);
                    parameters2.Add("@DocumentType", DName, DbType.String, ParameterDirection.Input);
                    _dapper.Insert(@"[dbo].[usp_AddGroupRegisterDocument]", parameters2);


                }

                DynamicParameters Fltparameters = new DynamicParameters();

                 Fltparameters.Add("@GroupHeadId", obj.Groups[0].UserId, DbType.Int32, ParameterDirection.Input);
              
                var FltIds = _dapper.GetMultipleObjects(@"[dbo].[GetAllGroupHeadFlighInformation]", Fltparameters, gr => gr.Read<GetFltIdsForGroupHead>(), gr => gr.Read<GetFltIdsForGroupHead>());

                if (FltIds.Item1.Count() > 0)
                {
                    #region
                    var reservationtable = new DataTable();
                    reservationtable.Columns.Add("RsvnTimeStamp", typeof(DateTime));
                    reservationtable.Columns.Add("RsvnPNR", typeof(string));
                    reservationtable.Columns.Add("AdultPNR", typeof(string));
                    reservationtable.Columns.Add("PaxName", typeof(string));
                    reservationtable.Columns.Add("PaxIDNum", typeof(string));
                    reservationtable.Columns.Add("PaxIDType", typeof(string));
                    reservationtable.Columns.Add("PaxIDExpiry", typeof(DateTime));
                    reservationtable.Columns.Add("PaxBDay", typeof(DateTime));
                    reservationtable.Columns.Add("PaxNationality", typeof(string));
                    reservationtable.Columns.Add("PaxGender", typeof(string));
                    reservationtable.Columns.Add("PaxCompany", typeof(string));
                    reservationtable.Columns.Add("CardNum", typeof(string));
                    reservationtable.Columns.Add("SeatNum", typeof(string));
                    reservationtable.Columns.Add("PaxDestination", typeof(int));
                    reservationtable.Columns.Add("PaxWT", typeof(int));
                    reservationtable.Columns.Add("BagWt", typeof(int));
                    reservationtable.Columns.Add("BagPcs", typeof(int));
                    reservationtable.Columns.Add("RsvnStatus", typeof(string));
                    reservationtable.Columns.Add("PaxMobNum", typeof(string));
                    reservationtable.Columns.Add("RsvEMail", typeof(string));
                    reservationtable.Columns.Add("SMSTimeStamp", typeof(DateTime));
                    reservationtable.Columns.Add("RsvRmks", typeof(string));
                    reservationtable.Columns.Add("RsvnAgent", typeof(int));
                    reservationtable.Columns.Add("RsvnChkAgent", typeof(int));
                    reservationtable.Columns.Add("RsvnChkTimeStamp", typeof(DateTime));
                    reservationtable.Columns.Add("PaxBoarded", typeof(int));
                    reservationtable.Columns.Add("ManifestColor", typeof(string));
                    reservationtable.Columns.Add("PaxTransitDest", typeof(int));
                    reservationtable.Columns.Add("FltID", typeof(int));
                    reservationtable.Columns.Add("UserId", typeof(int));
                    reservationtable.Columns.Add("GlobalFKId", typeof(int));

                    #endregion



                    for (int i = 0; i < FltIds.Item1.Count(); i++)
                    {

                        var RsvPNR = Guid.NewGuid().ToString("n").Substring(0, 6).ToString();

                        var FilterMembersByFltId = FltIds.Item2.Where(x => x.FltID == FltIds.Item1.ToList()[i].FltID).ToList();

                        var row = reservationtable.NewRow();
                        row["RsvnTimeStamp"] = DateTime.Now;
                        row["RsvnPNR"] = RsvPNR;
                        row["AdultPNR"] = ""; //YearMonthDiff(DateTime.Now, data2.Birthdate,FilterMembersByFltId);
                        row["PaxName"] = data2.Name;
                        row["PaxIDNum"] = data2.Documents;
                        row["PaxIDType"] = string.Join(",", obj.Groups[0].DocumentType);
                        row["PaxIDExpiry"] = data2.Birthdate.ToString("yyyy-MM-dd");
                        row["PaxBDay"] = data2.Birthdate.ToString("yyyy-MM-dd");
                        row["PaxNationality"] = data2.Nationality;
                        row["PaxGender"] = data2.Honorifics == "Mr" ? "M" : "F";
                        row["PaxCompany"] = DBNull.Value;
                        row["CardNum"] = DBNull.Value;
                        row["SeatNum"] = DBNull.Value;
                        row["PaxDestination"] = FltIds.Item1.ToList()[i].PaxDestination;
                        row["PaxWT"] = 0;
                        row["BagWt"] = 0;
                        row["BagPcs"] = 0;
                        row["RsvnStatus"] = "W";
                        row["PaxMobNum"] = data2.Mobile;
                        row["RsvEMail"] = data2.Email;
                        row["SMSTimeStamp"] = DBNull.Value;
                        row["RsvRmks"] = DBNull.Value;
                        row["RsvnAgent"] = DBNull.Value;
                        row["RsvnChkAgent"] = DBNull.Value;
                        row["RsvnChkTimeStamp"] = DBNull.Value;
                        row["PaxBoarded"] = DBNull.Value;
                        row["ManifestColor"] = "---";
                        row["PaxTransitDest"] = DBNull.Value;
                        row["FltID"] = FltIds.Item1.ToList()[i].FltID;
                        row["UserId"] = obj.Groups[0].UserId;
                        row["GlobalFKId"] = data2.Id;
                        reservationtable.Rows.Add(row);
                    }
                    DynamicParameters reservationParameter = new DynamicParameters();
                    reservationParameter.Add("@tv_GroupReservation", reservationtable.AsTableValuedParameter("dbo.tvGroupReservation"));
                    var dataReservation= _dapper.Insert<Reservation>(@"[dbo].[usp_AddGroupReserVationForOtherMembers]", reservationParameter);
                }

            }

            return Sendobj;
        }

        public Register AddOtherCorporateMember(Register obj)
        {
            DynamicParameters Checkparameters = new DynamicParameters();
            var table = new DataTable();
            table.Columns.Add("Email", typeof(string));
            table.Columns.Add("Mobile", typeof(string));
            table.Columns.Add("Passport", typeof(string));


            foreach (var item in obj.registerCorporate)
            {
                var row = table.NewRow();


                row["Email"] = string.IsNullOrEmpty(item.Email)
                    ? item.Mobile + "@dummy.ada.ae"
                    : item.Email;
                row["Mobile"] = item.Mobile;
                row["Passport"] = item.Documents;
                table.Rows.Add(row);

            }


            for (int i = 0; i < obj.registerCorporate.Count(); i++)
            {
                Checkparameters.Add("@Language", obj.registerCorporate[i].Language, DbType.String, ParameterDirection.Input);
                Checkparameters.Add("@Username", obj.Username, DbType.String, ParameterDirection.Input);
                Checkparameters.Add("@Email", string.IsNullOrEmpty(obj.registerCorporate[i].Email)
                    ? obj.registerCorporate[i].Mobile + "@dummy.ada.ae"
                    : obj.registerCorporate[i].Email, DbType.String, ParameterDirection.Input);
                Checkparameters.Add("@Documents", obj.registerCorporate[i].Documents, DbType.String, ParameterDirection.Input);
                Checkparameters.Add("@Birthdate", obj.registerCorporate[i].Birthdate, DbType.String, ParameterDirection.Input);
                Checkparameters.Add("@Mobile", obj.registerCorporate[i].Mobile, DbType.String, ParameterDirection.Input);
            }
            _dapper.Get<RegisterCorporate>(@"[dbo].[CheckUserAvailability]", Checkparameters);


            DynamicParameters parameters = new DynamicParameters();

            parameters.Add("@Language", obj.registerCorporate[0].Language, DbType.String, ParameterDirection.Input);
            parameters.Add("@Honorifics", obj.registerCorporate[0].Honorifics, DbType.String, ParameterDirection.Input);
            parameters.Add("@Name", obj.registerCorporate[0].Name, DbType.String, ParameterDirection.Input);
            parameters.Add("@Email", string.IsNullOrEmpty(obj.registerCorporate[0].Email)
                    ? obj.registerCorporate[0].Mobile + "@dummy.ada.ae"
                    : obj.registerCorporate[0].Email, DbType.String, ParameterDirection.Input);
            parameters.Add("@Nationality", obj.registerCorporate[0].Nationality, DbType.String, ParameterDirection.Input);
            parameters.Add("@Birthdate", obj.registerCorporate[0].Birthdate, DbType.DateTime, ParameterDirection.Input);
            parameters.Add("@Documents", obj.registerCorporate[0].Documents, DbType.String, ParameterDirection.Input);
            parameters.Add("@Mobile", obj.registerCorporate[0].Mobile, DbType.String, ParameterDirection.Input);
            parameters.Add("@IsDelmaIsland", obj.registerCorporate[0].IsDelmaIsland, DbType.Boolean, ParameterDirection.Input);
            parameters.Add("@IsUAEId", obj.registerCorporate[0].IsUAEId, DbType.Boolean, ParameterDirection.Input);
            parameters.Add("@CreatedOn", DateTime.Now, DbType.DateTime2, ParameterDirection.Input);
            parameters.Add("@Active", true, DbType.Boolean, ParameterDirection.Input);
            parameters.Add("@GovEntity", false, DbType.Boolean, ParameterDirection.Input);
            parameters.Add("@IsHead", obj.registerCorporate[0].IsHead, DbType.Boolean, ParameterDirection.Input);
            parameters.Add("@FKRegisterId", obj.registerCorporate[0].UserId, DbType.Int32, ParameterDirection.Input);

            var data2 = _dapper.Insert<RegisterGroup>(@"[dbo].[usp_AddCorporateInnerRegister]", parameters);


            if (data2 != null)
            {

                DynamicParameters parameters2 = new DynamicParameters();


                for (int j = 0; j < obj.registerCorporate[0].DocumentType.ToList().Count(); j++)

                {
                    string DName = obj.registerCorporate[0].DocumentType[j];

                    parameters2.Add("@RegisterFkId", obj.registerCorporate[0].UserId, DbType.Int32, ParameterDirection.Input);
                    parameters2.Add("@@RegisterCorporateFkId", data2.Id, DbType.Int32, ParameterDirection.Input);
                    parameters2.Add("@FilePathName", obj.registerCorporate[0].FileName[j], DbType.String, ParameterDirection.Input);
                    parameters2.Add("@DocumentType", DName, DbType.String, ParameterDirection.Input);
                    _dapper.Insert(@"[dbo].[usp_AddCorporateRegisterDocument]", parameters2);


                }


            }
            DynamicParameters Fltparameters = new DynamicParameters();

            Fltparameters.Add("@GroupHeadId", obj.registerCorporate[0].UserId, DbType.Int32, ParameterDirection.Input);

            var FltIds = _dapper.GetAll<GetFltIdsForGroupHead>(@"[dbo].[GetAllCorporateHeadFlighInformation]", Fltparameters);

            if (FltIds.Count() > 0)
            {
                #region
                var reservationtable = new DataTable();
                reservationtable.Columns.Add("RsvnTimeStamp", typeof(DateTime));
                reservationtable.Columns.Add("RsvnPNR", typeof(string));
                reservationtable.Columns.Add("AdultPNR", typeof(string));
                reservationtable.Columns.Add("PaxName", typeof(string));
                reservationtable.Columns.Add("PaxIDNum", typeof(string));
                reservationtable.Columns.Add("PaxIDType", typeof(string));
                reservationtable.Columns.Add("PaxIDExpiry", typeof(DateTime));
                reservationtable.Columns.Add("PaxBDay", typeof(DateTime));
                reservationtable.Columns.Add("PaxNationality", typeof(string));
                reservationtable.Columns.Add("PaxGender", typeof(string));
                reservationtable.Columns.Add("PaxCompany", typeof(string));
                reservationtable.Columns.Add("CardNum", typeof(string));
                reservationtable.Columns.Add("SeatNum", typeof(string));
                reservationtable.Columns.Add("PaxDestination", typeof(int));
                reservationtable.Columns.Add("PaxWT", typeof(int));
                reservationtable.Columns.Add("BagWt", typeof(int));
                reservationtable.Columns.Add("BagPcs", typeof(int));
                reservationtable.Columns.Add("RsvnStatus", typeof(string));
                reservationtable.Columns.Add("PaxMobNum", typeof(string));
                reservationtable.Columns.Add("RsvEMail", typeof(string));
                reservationtable.Columns.Add("SMSTimeStamp", typeof(DateTime));
                reservationtable.Columns.Add("RsvRmks", typeof(string));
                reservationtable.Columns.Add("RsvnAgent", typeof(int));
                reservationtable.Columns.Add("RsvnChkAgent", typeof(int));
                reservationtable.Columns.Add("RsvnChkTimeStamp", typeof(DateTime));
                reservationtable.Columns.Add("PaxBoarded", typeof(int));
                reservationtable.Columns.Add("ManifestColor", typeof(string));
                reservationtable.Columns.Add("PaxTransitDest", typeof(int));
                reservationtable.Columns.Add("FltID", typeof(int));
                reservationtable.Columns.Add("UserId", typeof(int));
                reservationtable.Columns.Add("GlobalFKId", typeof(int));

                #endregion



                for (int i = 0; i < FltIds.Count(); i++)
                {

                    var RsvPNR = Guid.NewGuid().ToString("n").Substring(0, 6).ToString();

                    var row = reservationtable.NewRow();
                    row["RsvnTimeStamp"] = DateTime.Now;
                    row["RsvnPNR"] = RsvPNR;
                    row["AdultPNR"] = "";
                    row["PaxName"] = data2.Name;
                    row["PaxIDNum"] = data2.Documents;
                    row["PaxIDType"] = string.Join(",", obj.registerCorporate[0].DocumentType);
                    row["PaxIDExpiry"] = data2.Birthdate.ToString("yyyy-MM-dd");
                    row["PaxBDay"] = data2.Birthdate.ToString("yyyy-MM-dd");
                    row["PaxNationality"] = data2.Nationality;
                    row["PaxGender"] = data2.Honorifics == "Mr" ? "M" : "F";
                    row["PaxCompany"] = DBNull.Value;
                    row["CardNum"] = DBNull.Value;
                    row["SeatNum"] = DBNull.Value;
                    row["PaxDestination"] = FltIds[i].PaxDestination;
                    row["PaxWT"] = 0;
                    row["BagWt"] = 0;
                    row["BagPcs"] = 0;
                    row["RsvnStatus"] = "W";
                    row["PaxMobNum"] = data2.Mobile;
                    row["RsvEMail"] = data2.Email;
                    row["SMSTimeStamp"] = DBNull.Value;
                    row["RsvRmks"] = DBNull.Value;
                    row["RsvnAgent"] = DBNull.Value;
                    row["RsvnChkAgent"] = DBNull.Value;
                    row["RsvnChkTimeStamp"] = DBNull.Value;
                    row["PaxBoarded"] = DBNull.Value;
                    row["ManifestColor"] = "---";
                    row["PaxTransitDest"] = DBNull.Value;
                    row["FltID"] = FltIds[i].FltID;
                    row["UserId"] = obj.registerCorporate[0].UserId;
                    row["GlobalFKId"] = data2.Id;
                    reservationtable.Rows.Add(row);
                }
                DynamicParameters reservationParameter = new DynamicParameters();
                reservationParameter.Add("@tv_GroupReservation", reservationtable.AsTableValuedParameter("dbo.tvGroupReservation"));
                var dataReservation = _dapper.Insert<Reservation>(@"[dbo].[usp_AddGroupReserVationForOtherMembers]", reservationParameter);
            }

            return new Register();
        }

        public Register update(Register obj)
        {



            var Dtype = obj.DocumentType[0].Split(',').ToList();

            string Name = string.Join(",", Dtype);

            var table = new DataTable();

            table.Columns.Add("RegisterFkId", typeof(int));
            table.Columns.Add("FilePathName", typeof(string));
            table.Columns.Add("DocumentType", typeof(string));


            for (int i = 0; i < obj.FileName.Count(); i++)
            {
                string DName = obj.DocumentType.ToList()[0].Split(',')[i];

                var row = table.NewRow();
                row["RegisterFkId"] = 0;
                row["FilePathName"] = obj.FileName[i];
                row["DocumentType"] = Convert.ToString(DName);
                table.Rows.Add(row);
            }



            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@tvDocumentType", table.AsTableValuedParameter("dbo.tvDocumentType"));
            parameters.Add("@Id", obj.Id, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@Honorifics", obj.Honorifics == null ? "" : obj.Honorifics, DbType.String, ParameterDirection.Input);
            parameters.Add("@Name", obj.Name == null ? "" : obj.Name, DbType.String, ParameterDirection.Input);
            parameters.Add("@Username", obj.Username == null ? "" : obj.Username, DbType.String, ParameterDirection.Input);
            //parameters.Add("@Password", obj.Password == null ? "" : obj.Password, DbType.String, ParameterDirection.Input);
            parameters.Add("@Email", string.IsNullOrEmpty(obj.Email)
                   ? obj.Mobile + "@dummy.ada.ae"
                   : obj.Email, DbType.String, ParameterDirection.Input);
            parameters.Add("@Nationality", obj.Nationality == null ? "" : obj.Nationality, DbType.String, ParameterDirection.Input);
            parameters.Add("@Birthdate", obj.Birthdate, DbType.DateTime, ParameterDirection.Input);
            parameters.Add("@Documents", obj.Documents == null ? "" : obj.Documents, DbType.String, ParameterDirection.Input);
            //parameters.Add("@DocumentType", "" == null ? "" : "", DbType.String, ParameterDirection.Input);
            parameters.Add("@Mobile", obj.Mobile == null ? "" : obj.Mobile, DbType.String, ParameterDirection.Input);
            parameters.Add("@CreatedOn", obj.CreatedOn, DbType.DateTime, ParameterDirection.Input);
            parameters.Add("@Active", obj.Active, DbType.Boolean, ParameterDirection.Input);
            parameters.Add("@IsDelmaIsland", obj.IsDelmaIsland, DbType.Boolean, ParameterDirection.Input);
            parameters.Add("@IsUAEId", obj.IsUAEId, DbType.Boolean, ParameterDirection.Input);


            var data = _dapper.Insert<Register>(@"[dbo].[usp_UpdateRegister]", parameters);

            return data;
        }

        public Register updateGroup(Register obj)
        {

           

            var Dtype = obj.DocumentType[0].Split(',').ToList();

            string Name = string.Join(",", Dtype);

            var table = new DataTable();

            table.Columns.Add("RegisterGroupFkId", typeof(int));
            table.Columns.Add("FilePathName", typeof(string));
            table.Columns.Add("DocumentType", typeof(string));


            for (int i = 0; i < obj.FileName.Count(); i++)
            {
                string DName = obj.DocumentType.ToList()[0].Split(',')[i];

                var row = table.NewRow();
                row["RegisterGroupFkId"] = obj.Id;
                row["FilePathName"] = obj.FileName[i];
                row["DocumentType"] = Convert.ToString(DName);
                table.Rows.Add(row);
            }



            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@tv_RegisterGroup", table.AsTableValuedParameter("dbo.tvRegisterGroup"));
            parameters.Add("@Id", obj.Id, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@Honorifics", obj.Honorifics == null ? "" : obj.Honorifics, DbType.String, ParameterDirection.Input);
            parameters.Add("@Name", obj.Name == null ? "" : obj.Name, DbType.String, ParameterDirection.Input);
            if (string.IsNullOrEmpty(obj.Email)) {
                parameters.Add("@Email", obj.Mobile + "@dummy.ada.ae", DbType.String, ParameterDirection.Input);
            }
            else
            {
                parameters.Add("@Email", obj.Email.Contains("@dummy.ada.ae") ? obj.Mobile + "@dummy.ada.ae" : obj.Email, DbType.String, ParameterDirection.Input);
                
            }
            parameters.Add("@Nationality", obj.Nationality == null ? "" : obj.Nationality, DbType.String, ParameterDirection.Input);
            parameters.Add("@Birthdate", obj.Birthdate, DbType.DateTime, ParameterDirection.Input);
            parameters.Add("@Documents", obj.Documents == null ? "" : obj.Documents, DbType.String, ParameterDirection.Input);
            parameters.Add("@Mobile", obj.Mobile == null ? "" : obj.Mobile, DbType.String, ParameterDirection.Input);
            parameters.Add("@Active", obj.Active, DbType.Boolean, ParameterDirection.Input);
            parameters.Add("@IsDelmaIsland", obj.IsDelmaIsland, DbType.Boolean, ParameterDirection.Input);
            parameters.Add("@IsHead", obj.IsHead, DbType.Boolean, ParameterDirection.Input);
            parameters.Add("@IsUAEId", obj.IsUAEId, DbType.Boolean, ParameterDirection.Input);
            parameters.Add("@GovEntity", obj.GovEntity, DbType.Boolean, ParameterDirection.Input);


            var data = _dapper.Insert<Register>(@"[dbo].[usp_UpdateGroup]", parameters);

            return data;
        }

        public Register updateCorporate(Register obj)
        {

            
            var Dtype = obj.DocumentType[0].Split(',').ToList();

            string Name = string.Join(",", Dtype);

            var table = new DataTable();

            table.Columns.Add("RegisterCorporateFkId", typeof(int));
            table.Columns.Add("FilePathName", typeof(string));
            table.Columns.Add("DocumentType", typeof(string));


            for (int i = 0; i < obj.FileName.Count(); i++)
            {
                string DName = obj.DocumentType.ToList()[0].Split(',')[i];

                var row = table.NewRow();
                row["RegisterCorporateFkId"] = obj.Id;
                row["FilePathName"] = obj.FileName[i];
                row["DocumentType"] = Convert.ToString(DName);
                table.Rows.Add(row);
            }



            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@tv_RegisterCorporate", table.AsTableValuedParameter("dbo.tvRegisterCorporate"));
            parameters.Add("@Id", obj.Id, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@Honorifics", obj.Honorifics == null ? "" : obj.Honorifics, DbType.String, ParameterDirection.Input);
            parameters.Add("@Name", obj.Name == null ? "" : obj.Name, DbType.String, ParameterDirection.Input);
            if (string.IsNullOrEmpty(obj.Email))
            {
                parameters.Add("@Email", obj.Mobile + "@dummy.ada.ae", DbType.String, ParameterDirection.Input);
            }
            else
            {
                parameters.Add("@Email", obj.Email.Contains("@dummy.ada.ae") ? obj.Mobile + "@dummy.ada.ae" : obj.Email, DbType.String, ParameterDirection.Input);

            }
            parameters.Add("@Nationality", obj.Nationality == null ? "" : obj.Nationality, DbType.String, ParameterDirection.Input);
            parameters.Add("@Birthdate", obj.Birthdate, DbType.DateTime, ParameterDirection.Input);
            parameters.Add("@Documents", obj.Documents == null ? "" : obj.Documents, DbType.String, ParameterDirection.Input);
            parameters.Add("@Mobile", obj.Mobile == null ? "" : obj.Mobile, DbType.String, ParameterDirection.Input);
            parameters.Add("@Active", obj.Active, DbType.Boolean, ParameterDirection.Input);
            parameters.Add("@IsDelmaIsland", obj.IsDelmaIsland, DbType.Boolean, ParameterDirection.Input);
            parameters.Add("@IsHead", obj.IsHead, DbType.Boolean, ParameterDirection.Input);
            parameters.Add("@IsUAEId", obj.IsUAEId, DbType.Boolean, ParameterDirection.Input);
            parameters.Add("@GovEntity", obj.GovEntity, DbType.Boolean, ParameterDirection.Input);


            var data = _dapper.Insert<Register>(@"[dbo].[usp_UpdateCorporate]", parameters);

            return data;
        }


        public Register GetDocumentByUserId(int Id)
        {
            DynamicParameters parameters = new DynamicParameters();

            parameters.Add("@Id", Id, DbType.Int32, ParameterDirection.Input);

            var data = _dapper.Get<Register>(@"[dbo].[GetDocumentByUserId]", parameters);

            return data;
        }


        public Register GetGroupDocumentByUserId(int Id)
        {
            DynamicParameters parameters = new DynamicParameters();

            parameters.Add("@Id", Id, DbType.Int32, ParameterDirection.Input);

            var data = _dapper.Get<Register>(@"[dbo].[GetGroupDocumentByUserId]", parameters);

            return data;
        }

        public List<Register> GetALLGroupMemberByGroupHeadId(int Id)
        {
            DynamicParameters parameters = new DynamicParameters();

            parameters.Add("@Id", Id, DbType.Int32, ParameterDirection.Input);

            var data = _dapper.GetAll<Register>(@"[dbo].[GetALLGroupMemberByGroupHeadId]", parameters);

            return data;
        }

        public List<Register> GetALLCorporateMemberByCorporateHeadId(int Id)
        {
            DynamicParameters parameters = new DynamicParameters();

            parameters.Add("@Id", Id, DbType.Int32, ParameterDirection.Input);

            var data = _dapper.GetAll<Register>(@"[dbo].[GetALLCorporateMemberByCorporateHeadId]", parameters);

            return data;
        }

        public Register GetCorporateDocumentByUserId(int Id)
        {
            DynamicParameters parameters = new DynamicParameters();

            parameters.Add("@Id", Id, DbType.Int32, ParameterDirection.Input);

            var data = _dapper.Get<Register>(@"[dbo].[GetCorporateDocumentByUserId]", parameters);

            return data;
        }

        public CodeVerification ForgotPassword(CodeVerification obj)
        {

            DynamicParameters parameters = new DynamicParameters();

            parameters.Add("@Email", obj.Email == null ? "" : obj.Email, DbType.String, ParameterDirection.Input);
            parameters.Add("@Mobile", obj.Mobile == null ? "" : obj.Mobile, DbType.String, ParameterDirection.Input);
            parameters.Add("@Action", obj.Action == null ? "" : obj.Action, DbType.String, ParameterDirection.Input);


            var data = _dapper.Get<CodeVerification>(@"[dbo].[ForgotPassword]", parameters);

            return data;
        }

        public object ResetPassword(CodeVerification obj)
        {


            DynamicParameters parameters = new DynamicParameters();

            parameters.Add("@VerifyCode", obj.VerifyCode, DbType.String, ParameterDirection.Input);
            parameters.Add("@Email", obj.Email == null ? "" : obj.Email, DbType.String, ParameterDirection.Input);
            parameters.Add("@Mobile", obj.Mobile == null ? "" : obj.Mobile, DbType.String, ParameterDirection.Input);
            parameters.Add("@Action", obj.Action == null ? "" : obj.Action, DbType.String, ParameterDirection.Input);
            parameters.Add("@Password", obj.Password, DbType.String, ParameterDirection.Input);



            var data = _dapper.Insert<Register>(@"[dbo].[usp_ResetPassword]", parameters);

            return data;
        }
       

        public Register ActiveInactiveByUserId(int Id)
        {
            DynamicParameters parameters = new DynamicParameters();

            parameters.Add("@UserId", Id, DbType.Int32, ParameterDirection.Input);

            var data = _dapper.Get<Register>(@"[dbo].[UserActiveInactiveByUserID]", parameters);

            return data;
        }

        public string YearMonthDiff(DateTime startDate, DateTime endDate, List<GetFltIdsForGroupHead> obj)
        {
            int monthDiff = ((endDate.Year * 12) + endDate.Month) - ((startDate.Year * 12) + startDate.Month) + 1;
            int years = (int)Math.Floor((decimal)(monthDiff / 12));
            int months = monthDiff % 12;


            if (years == 0 && months <= 6)
            {


               return obj.FirstOrDefault().RsvnPNR;


            }

            return "";


        }

    }


}
