using ADA.API.DBManager;
using ADA.API.IRepositories;
using ADAClassLibrary;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace ADA.API.Repositories
{
    public class ReservationRepositery : IReservationRepositery
    {
        private readonly IDapper _dapper;

        public ReservationRepositery(IDapper dapper)
        {
            _dapper = dapper;
        }

        List<string> objList = new List<string>();
        List<Reservation> MemberListObj = new List<Reservation>();
        public int AdultCount = 0;
        public int InfantCount = 0;


        public List<Reservation> Add(Reservation obj)
        {


            List<Reservation> ListObj = new List<Reservation>();
            DynamicParameters parameters = new DynamicParameters();
            DynamicParameters MemberCheckparametersForInfant = new DynamicParameters();

            var result = obj.Groupreservation.OrderBy(x => x.PaxBDay).ToList();

            foreach (var item in result)
            {
                Check_Infant_Adult_Count(Convert.ToDateTime(item.PaxBDay), DateTime.Now);
            }

            if (AdultCount >= InfantCount)
            {

                var table = new DataTable();
                table.Columns.Add("FltID", typeof(int));
                table.Columns.Add("UserId", typeof(int));
                table.Columns.Add("RGFKId", typeof(int));



                foreach (var item in result)
                {
                    var row = table.NewRow();
                    row["FltID"] = item.FltID;
                    row["UserId"] = item.UserId;
                    row["RGFKId"] = item.GlobalFKId;
                    table.Rows.Add(row);

                }

                DynamicParameters UserAlreadyExists = new DynamicParameters();
                UserAlreadyExists.Add("@Lang", obj.Lang, DbType.String, ParameterDirection.Input);
                UserAlreadyExists.Add("@tvCheckUserAlreadyExists", table.AsTableValuedParameter("dbo.tvCheckUserAlreadyExists"));

                var UserCheckBool = _dapper.Insert<string>(@"[dbo].[usp_UserExistsGroupReserVation]", UserAlreadyExists);


                if (UserCheckBool == "true")
                {


                    // For Infant to get Adult PNR
                    MemberCheckparametersForInfant.Add("@FltID", result.FirstOrDefault().FltID, DbType.Int32, ParameterDirection.Input);
                    MemberCheckparametersForInfant.Add("@UserId", result.FirstOrDefault().UserId, DbType.Int32, ParameterDirection.Input);

                    var MemberFullData = _dapper.GetAll<Reservation>(@"[dbo].[usp_GetAllGroupReserVationByFltIdandUserId]", MemberCheckparametersForInfant);

                    foreach (var item in MemberFullData)
                    {
                        MemberListObj.Add(item);
                    }

                    //YearMonthDiff(Convert.ToDateTime(item.PaxBDay), DateTime.Now, RsvPNR, item.Lang)

                    foreach (var item in result)
                    {

                        var RsvPNR = Guid.NewGuid().ToString("n").Substring(0, 6).ToString();


                        string DocType = string.Join(",", item.PaxIDType);

                        parameters.Add("@RsvnTimeStamp", DateTime.Now, DbType.DateTime, ParameterDirection.Input);
                        parameters.Add("@RsvnPNR", RsvPNR, DbType.String, ParameterDirection.Input);
                        parameters.Add("@AdultPNR", "", DbType.String, ParameterDirection.Input);
                        parameters.Add("@PaxName", item.PaxName, DbType.String, ParameterDirection.Input);
                        parameters.Add("@PaxIDNum", item.PaxIDNum, DbType.String, ParameterDirection.Input);
                        parameters.Add("@PaxIDType", DocType, DbType.String, ParameterDirection.Input);
                        parameters.Add("@PaxIDExpiry", DateTime.Now, DbType.DateTime, ParameterDirection.Input);
                        parameters.Add("@PaxBDay", item.PaxBDay, DbType.DateTime, ParameterDirection.Input);
                        parameters.Add("@PaxNationality", item.PaxNationality, DbType.String, ParameterDirection.Input);
                        parameters.Add("@PaxGender", item.PaxGender == "Mr" ? "M" : "F", DbType.String, ParameterDirection.Input);
                        parameters.Add("@PaxCompany", DBNull.Value, DbType.String, ParameterDirection.Input);
                        parameters.Add("@CardNum", DBNull.Value, DbType.String, ParameterDirection.Input);
                        parameters.Add("@SeatNum", DBNull.Value, DbType.String, ParameterDirection.Input);
                        parameters.Add("@PaxDestination", item.PaxDestination, DbType.Int32, ParameterDirection.Input);
                        parameters.Add("@PaxWT", item.PaxWT, DbType.Int32, ParameterDirection.Input);
                        parameters.Add("@BagWt", item.BagWt, DbType.Int32, ParameterDirection.Input);
                        parameters.Add("@BagPcs", item.BagPcs, DbType.Int32, ParameterDirection.Input);
                        parameters.Add("@RsvnStatus", item.RsvnStatus, DbType.Int32, ParameterDirection.Input);
                        parameters.Add("@PaxMobNum", item.PaxMobNum, DbType.String, ParameterDirection.Input);
                        parameters.Add("@RsvEMail", item.RsvEMail, DbType.String, ParameterDirection.Input);
                        parameters.Add("@SMSTimeStamp", DBNull.Value, DbType.DateTime, ParameterDirection.Input);
                        parameters.Add("@RsvRmks", DBNull.Value, DbType.String, ParameterDirection.Input);
                        parameters.Add("@RsvnAgent", DBNull.Value, DbType.Int32, ParameterDirection.Input);
                        parameters.Add("@RsvnChkAgent", DBNull.Value, DbType.Int32, ParameterDirection.Input);
                        parameters.Add("@RsvnChkTimeStamp", DBNull.Value, DbType.DateTime, ParameterDirection.Input);
                        parameters.Add("@PaxBoarded", DBNull.Value, DbType.Int32, ParameterDirection.Input);
                        parameters.Add("@ManifestColor", "---", DbType.String, ParameterDirection.Input);
                        parameters.Add("@PaxTransitDest", DBNull.Value, DbType.Int32, ParameterDirection.Input);
                        parameters.Add("@FltID", item.FltID, DbType.Int32, ParameterDirection.Input);
                        parameters.Add("@UserId", item.UserId, DbType.Int32, ParameterDirection.Input);
                        parameters.Add("@GlobalFKId", item.GlobalFKId, DbType.Int32, ParameterDirection.Input);
                        parameters.Add("@Lang", item.Lang, DbType.String, ParameterDirection.Input);
                        parameters.Add("@CreatedBy", item.CreatedBy, DbType.Int32, ParameterDirection.Input);
                        parameters.Add("@ActionSource", item.ActionSource, DbType.String, ParameterDirection.Input);

                        var Data = _dapper.Insert<Reservation>(@"[dbo].[usp_AddGroupReserVation]", parameters);

                        ListObj.Add(Data);

                    }

                }
                //For SMS
                DynamicParameters parameters3 = new DynamicParameters();
                parameters3.Add("@FltID", ListObj.FirstOrDefault().FltID, DbType.Int32, ParameterDirection.Input);
                parameters3.Add("@UserId", ListObj.FirstOrDefault().UserId, DbType.Int32, ParameterDirection.Input);
                var FullData = _dapper.GetAll<Reservation>(@"[dbo].[usp_GetAllGroupReserVationByFltIdandUserId]", parameters3);

                return FullData;
            }

            else
            {

                DynamicParameters CheckSingleInfant = new DynamicParameters();
                CheckSingleInfant.Add("@FltID", result.FirstOrDefault().FltID, DbType.Int32, ParameterDirection.Input);
                CheckSingleInfant.Add("@UserId", result.FirstOrDefault().UserId, DbType.Int32, ParameterDirection.Input);
                var ExitsAdult = _dapper.Insert<string>(@"[dbo].[CheckSingleInfantReservation]", CheckSingleInfant);

                if (ExitsAdult == "true")
                {

                    var table = new DataTable();
                    table.Columns.Add("FltID", typeof(int));
                    table.Columns.Add("UserId", typeof(int));
                    table.Columns.Add("RGFKId", typeof(int));



                    foreach (var item in result)
                    {
                        var row = table.NewRow();
                        row["FltID"] = item.FltID;
                        row["UserId"] = item.UserId;
                        row["RGFKId"] = item.GlobalFKId;
                        table.Rows.Add(row);

                    }

                    DynamicParameters UserAlreadyExists = new DynamicParameters();
                    UserAlreadyExists.Add("@Lang", obj.Lang, DbType.String, ParameterDirection.Input);
                    UserAlreadyExists.Add("@tvCheckUserAlreadyExists", table.AsTableValuedParameter("dbo.tvCheckUserAlreadyExists"));

                    var UserCheckBool = _dapper.Insert<string>(@"[dbo].[usp_UserExistsGroupReserVation]", UserAlreadyExists);


                    if (UserCheckBool == "true")
                    {


                        // For Infant to get Adult PNR
                        MemberCheckparametersForInfant.Add("@FltID", result.FirstOrDefault().FltID, DbType.Int32, ParameterDirection.Input);
                        MemberCheckparametersForInfant.Add("@UserId", result.FirstOrDefault().UserId, DbType.Int32, ParameterDirection.Input);

                        var MemberFullData = _dapper.GetAll<Reservation>(@"[dbo].[usp_GetAllGroupReserVationByFltIdandUserId]", MemberCheckparametersForInfant);

                        foreach (var item in MemberFullData)
                        {
                            MemberListObj.Add(item);
                        }

                        //YearMonthDiff(Convert.ToDateTime(item.PaxBDay), DateTime.Now, RsvPNR, item.Lang)

                        foreach (var item in result)
                        {

                            var RsvPNR = Guid.NewGuid().ToString("n").Substring(0, 6).ToString();


                            string DocType = string.Join(",", item.PaxIDType);

                            parameters.Add("@RsvnTimeStamp", DateTime.Now, DbType.DateTime, ParameterDirection.Input);
                            parameters.Add("@RsvnPNR", RsvPNR, DbType.String, ParameterDirection.Input);
                            parameters.Add("@AdultPNR", "", DbType.String, ParameterDirection.Input);
                            parameters.Add("@PaxName", item.PaxName, DbType.String, ParameterDirection.Input);
                            parameters.Add("@PaxIDNum", item.PaxIDNum, DbType.String, ParameterDirection.Input);
                            parameters.Add("@PaxIDType", DocType, DbType.String, ParameterDirection.Input);
                            parameters.Add("@PaxIDExpiry", DateTime.Now, DbType.DateTime, ParameterDirection.Input);
                            parameters.Add("@PaxBDay", item.PaxBDay, DbType.DateTime, ParameterDirection.Input);
                            parameters.Add("@PaxNationality", item.PaxNationality, DbType.String, ParameterDirection.Input);
                            parameters.Add("@PaxGender", item.PaxGender == "Mr" ? "M" : "F", DbType.String, ParameterDirection.Input);
                            parameters.Add("@PaxCompany", DBNull.Value, DbType.String, ParameterDirection.Input);
                            parameters.Add("@CardNum", DBNull.Value, DbType.String, ParameterDirection.Input);
                            parameters.Add("@SeatNum", DBNull.Value, DbType.String, ParameterDirection.Input);
                            parameters.Add("@PaxDestination", item.PaxDestination, DbType.Int32, ParameterDirection.Input);
                            parameters.Add("@PaxWT", item.PaxWT, DbType.Int32, ParameterDirection.Input);
                            parameters.Add("@BagWt", item.BagWt, DbType.Int32, ParameterDirection.Input);
                            parameters.Add("@BagPcs", item.BagPcs, DbType.Int32, ParameterDirection.Input);
                            parameters.Add("@RsvnStatus", item.RsvnStatus, DbType.Int32, ParameterDirection.Input);
                            parameters.Add("@PaxMobNum", item.PaxMobNum, DbType.String, ParameterDirection.Input);
                            parameters.Add("@RsvEMail", item.RsvEMail, DbType.String, ParameterDirection.Input);
                            parameters.Add("@SMSTimeStamp", DBNull.Value, DbType.DateTime, ParameterDirection.Input);
                            parameters.Add("@RsvRmks", DBNull.Value, DbType.String, ParameterDirection.Input);
                            parameters.Add("@RsvnAgent", DBNull.Value, DbType.Int32, ParameterDirection.Input);
                            parameters.Add("@RsvnChkAgent", DBNull.Value, DbType.Int32, ParameterDirection.Input);
                            parameters.Add("@RsvnChkTimeStamp", DBNull.Value, DbType.DateTime, ParameterDirection.Input);
                            parameters.Add("@PaxBoarded", DBNull.Value, DbType.Int32, ParameterDirection.Input);
                            parameters.Add("@ManifestColor", "---", DbType.String, ParameterDirection.Input);
                            parameters.Add("@PaxTransitDest", DBNull.Value, DbType.Int32, ParameterDirection.Input);
                            parameters.Add("@FltID", item.FltID, DbType.Int32, ParameterDirection.Input);
                            parameters.Add("@UserId", item.UserId, DbType.Int32, ParameterDirection.Input);
                            parameters.Add("@GlobalFKId", item.GlobalFKId, DbType.Int32, ParameterDirection.Input);
                            parameters.Add("@Lang", item.Lang, DbType.String, ParameterDirection.Input);
                            parameters.Add("@CreatedBy", item.CreatedBy, DbType.Int32, ParameterDirection.Input);
                            parameters.Add("@ActionSource", item.ActionSource, DbType.String, ParameterDirection.Input);

                            var Data = _dapper.Insert<Reservation>(@"[dbo].[usp_AddGroupReserVation]", parameters);

                            ListObj.Add(Data);

                        }

                    }
                    //For SMS
                    DynamicParameters parameters3 = new DynamicParameters();
                    parameters3.Add("@FltID", ListObj.FirstOrDefault().FltID, DbType.Int32, ParameterDirection.Input);
                    parameters3.Add("@UserId", ListObj.FirstOrDefault().UserId, DbType.Int32, ParameterDirection.Input);
                    var FullData = _dapper.GetAll<Reservation>(@"[dbo].[usp_GetAllGroupReserVationByFltIdandUserId]", parameters3);

                    return FullData;

                }
                else
                {


                    throw new Exception("Number of infants must be less or equals to toatal Adults.");
                }
            }


        }
        public List<Reservation> GetAll()
        {
            DynamicParameters parameters = new DynamicParameters();

            var data = _dapper.GetAll<Reservation>(@"[dbo].[GetAllReservationSchedule]", parameters);
            return data;
        }

        public Reservation GetByID(int id)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@FltID", id, DbType.Int32, ParameterDirection.Input);
            return _dapper.Get<Reservation>(@"[dbo].[usp_getReservationByID]", parameters);
        }

        public object GetReservationsDropDown()
        {
            DynamicParameters parameters = new DynamicParameters();
            var data = _dapper.GetMultipleObjects("[dbo].[usp_GetValuesDropDown]", parameters, gr => gr.Read<Destination>(), gr => gr.Read<AirCraftDropDown>(), qr => qr.Read<Pilot>(), pr => pr.Read<Staff>(), jr => jr.Read<Customer>());

            DropdownList obj = new DropdownList();

            obj.Destination = data.Item1.ToList();
            obj.AirCraft = data.Item2.Where(x => x.ACType.Contains("FW")).ToList();
            obj.Pilot = data.Item3.Where(x => x.ACType.Contains("FW")).ToList();
            obj.Staff = data.Item4.Where(x => x.StaffActive == true || x.StaffGrp.Contains("FW") || x.StaffGrp.Contains("CM") || x.StaffGrp.Contains("AL"))?.ToList();
            obj.Customer = data.Item5.ToList();


            return obj;
        }



        public Reservation Update(Reservation obj)
        {
            DynamicParameters parameters = new DynamicParameters();

            return _dapper.Update<Reservation>(@"[dbo].[usp_updateReservation]", parameters);
        }




        public static string GetData(DateTime Date, string ParentPNR)
        {


            DateTime today = DateTime.Now;

            DateTime dob = Convert.ToDateTime(Date);
            TimeSpan ts = today - dob;
            DateTime age = DateTime.MinValue + ts;
            int years = age.Year - 1;
            int months = age.Month - 1;
            int days = age.Day - 1;
            if (years <= 1 && months <= 12)
            {
                return ParentPNR;
            }

            return "";
        }

        public List<MyBookingDetails> GetMyBookingDetailsByID(int id)
        {
            DynamicParameters parameters = new DynamicParameters();

            parameters.Add("@UserId", id, DbType.Int32, ParameterDirection.Input);

            return _dapper.GetAll<MyBookingDetails>(@"[dbo].[GetMyBookingDetailsByUserId]", parameters);
        }

        public List<MyBookingDetails> GetCancelBookingDetailsByUserId(int id)
        {
            DynamicParameters parameters = new DynamicParameters();

            parameters.Add("@UserId", id, DbType.Int32, ParameterDirection.Input);

            return _dapper.GetAll<MyBookingDetails>(@"[dbo].[GetCancelBookingDetailsByUserId]", parameters);
        }
        public List<MyBookingDetails> CancelBookingByRsvnId(Reservation obj)
        {
            DynamicParameters parameters = new DynamicParameters();

            parameters.Add("@Lang", obj.Lang, DbType.String, ParameterDirection.Input);
            parameters.Add("@RsvnId", obj.RsvnID, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@UpdatedActionSource", obj.UpdatedActionSource, DbType.String, ParameterDirection.Input);
            parameters.Add("@UpdatedBy", obj.UpdatedBy, DbType.Int32, ParameterDirection.Input);

            return _dapper.GetAll<MyBookingDetails>(@"[dbo].[CancelBooking]", parameters);
        }
        public List<MyBookingDetails> ReBookingByRsvnId(Reservation obj)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@Lang", obj.Lang, DbType.String, ParameterDirection.Input);
            parameters.Add("@RsvnId", obj.RsvnID, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@UpdatedActionSource", obj.UpdatedActionSource, DbType.String, ParameterDirection.Input);
            parameters.Add("@UpdatedBy", obj.UpdatedBy, DbType.Int32, ParameterDirection.Input);


            return _dapper.GetAll<MyBookingDetails>(@"[dbo].[ReBookingByRsvnId]", parameters);
        }

        public List<BordingPassDetails> GetBordingPassDetails(string PNR)
        {
            DynamicParameters parameters = new DynamicParameters();

            parameters.Add("@RsvnPNR", PNR, DbType.String, ParameterDirection.Input);

            return _dapper.GetAll<BordingPassDetails>(@"[dbo].[usp_CheckInBordingPassDetailsByUserId]", parameters);
        }


        public string YearMonthDiff(DateTime startDate, DateTime endDate, string PNR, string lang)
        {
            int monthDiff = ((endDate.Year * 12) + endDate.Month) - ((startDate.Year * 12) + startDate.Month) + 1;
            int years = (int)Math.Floor((decimal)(monthDiff / 12));
            int months = monthDiff % 12;


            if (years == 0 && months <= 6)
            {

                if (objList.Count > 0)
                {

                    return objList.First();

                }
                else
                {

                    if (MemberListObj.Count > 0)
                    {

                        List<string> PNRAdultObj = new List<string>();

                        foreach (var item in MemberListObj)
                        {

                            int AdultmonthDiff = ((DateTime.Now.Year * 12) + DateTime.Now.Month) - ((Convert.ToDateTime(item.PaxBDay).Year * 12) + Convert.ToDateTime(item.PaxBDay).Month) + 1;
                            int Adultyears = (int)Math.Floor((decimal)(AdultmonthDiff / 12));
                            int Adultmonths = monthDiff % 12;


                            if (AdultmonthDiff > 6)
                            {

                                PNRAdultObj.Add(item.RsvnPNR);
                            }

                        }

                        return PNRAdultObj.First();

                    }

                    else
                    {
                        if (lang == "ar")
                        {
                            throw new Exception("يرجى تحديد الشخص البالغ المرافق أولاً قبل الرضيع.");
                        }
                        else
                        {
                            throw new Exception("Please select the accompanying adult first before the infant.");
                        }

                    }


                }


            }
            else
            {
                objList.Add(PNR);

            }


            return "";



        }


        public void Check_Infant_Adult_Count(DateTime startDate, DateTime endDate)
        {
            int monthDiff = ((endDate.Year * 12) + endDate.Month) - ((startDate.Year * 12) + startDate.Month) + 1;
            int years = (int)Math.Floor((decimal)(monthDiff / 12));
            int months = monthDiff % 12;


            if (years == 0 && months <= 6)
            {

                InfantCount++;


            }
            else
            {
                AdultCount++;

            }

        }


    }
}
