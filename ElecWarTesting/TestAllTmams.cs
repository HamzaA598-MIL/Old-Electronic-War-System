//using ElecWarSystem.Controllers;
//using ElecWarSystem.Data;
//using ElecWarSystem.Models;
//using ElecWarSystem.Models.IModel;
//using ElecWarSystem.Serivces;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using System;
//using System.Collections.Generic;
//using System.Linq;

//namespace ElecWarTesting
//{
//    [TestClass]
//    public class TestAllTmams
//    {
//        public void GeneratePersons(int unitID, int num, int min, int max)
//        {
//            PersonService personService = new PersonService();
//            String[] Names = new String[]
//            {
//                "محمد",
//                "محمود",
//                "عبد الله",
//                "عبد الوهاب",
//                "سراج الدين",
//                "أحمد",
//                "شهاب",
//                "أيمن",
//                "مصطفى",
//                "فارس",
//                "حامد",
//                "سيد",
//                "إسلام",
//                "شذلى",
//                "عادل",
//                "نجيب",
//                "عمر",
//                "عمرو",
//                "شاهين",
//                "حسين",
//                "شريف",
//                "سمير",
//                "باسم",
//                "على",
//                "خالد"
//            };
//            Random random = new Random();

//            for (int i = 0; i < num; i++)
//            {
//                String Name = "";
//                for (int j = 0; j < 4; j++)
//                {
//                    Name += $"{Names[random.Next(0, Names.Length)]} ";
//                }
//                Person person = new Person()
//                {
//                    UnitID = unitID,
//                    FullName = Name.Trim(),
//                    RankID = random.Next(min, max),
//                    MilID = random.Next(20220000, 20230000).ToString(),
//                };
//                personService.Add(person);
//            }
//        }
//        //[TestMethod]
//        //public void TestMethod1()
//        //{
//        //    //UserService userService = new UserService();
//        //    //int unitId = 7;
//        //    //User user = new User
//        //    //{
//        //    //    UserName = "fog 702",
//        //    //    Password = "702",
//        //    //    UnitID = unitId
//        //    //};
//        //    //userService.CreateNewUser(user, "702");
//        //    //GeneratePersons(unitId, 3, 3, 6);
//        //    //GeneratePersons(unitId, 13, 7, 13);
//        //    //GeneratePersons(unitId, 30, 14, 18);
//        //    //GeneratePersons(unitId, 180, 19, 21);
//        //}
//        [TestMethod]
//        public void TestVacationService()
//        {
//            TmamService tmamService = new TmamService();
//            AppDBContext appDBContext = new AppDBContext();
//            VacationService vacationService = new VacationService();
//            List<Tmam> tmamList = new List<Tmam>();
//            VacationDetail vacationDetail = new VacationDetail()
//            {
//                PersonID = 232,
//                DateFrom = new DateTime(2023, 1, 14),
//                DateTo = new DateTime(2023, 1, 18),
//                VacationType = "راحة",
//            };
            
//            for (int i = 0; i < 10; i++)
//            {
//                Tmam tmam = new Tmam()
//                {
//                    UnitID = 4,
//                    Date = DateTime.Today.AddDays(i),
//                };

//                long tmamID = tmamService.AddTmam(tmam);
//                vacationService.Add(
//                    new Vacation()
//                    {
//                        VacationDetail = vacationDetail,
//                        TmamID = tmamID,
//                        Tmam = tmam
//                    });
//                tmamList.Add(tmam);
//            }

//            long hdID = vacationService.GetID(vacationDetail);
//            int Expected = 4;
//            int Actual = vacationService.GetCount(hdID);

//            Assert.AreEqual(Expected, Actual);
//            Expected = 1;
//            Actual = appDBContext.VacationDetails.Where(row => row.PersonID == vacationDetail.PersonID && row.DateFrom >= vacationDetail.DateFrom && row.DateTo <= vacationDetail.DateTo).Count();
//            Assert.AreEqual(Expected, Actual);
//            //check person status before clean
//            Expected = (int)TmamEnum.Vacation;
//            Actual = appDBContext.Persons.Find(vacationDetail.PersonID).Status;
//            Assert.AreEqual(Expected, Actual);

//            //cleanup
//            //vacationService.Delete(vacationDetail.ID);
//            //for (int i = 0; i < 5; i++)
//            //{
//            //    tmamService.DeleteTmam(tmamList[i]);
//            //}
//        }
//        [TestMethod]
//        public void TestHospitalService()
//        {
//            TmamService tmamService = new TmamService();
//            AppDBContext appDBContext = new AppDBContext();
//            HospitalService hospitalService = new HospitalService();
//            List<Tmam> tmamList = new List<Tmam>();
            
//            HospitalDetails hospitalDetails = new HospitalDetails()
//            {
//                PersonID = 232,
//                DateFrom = new DateTime(2022, 11, 5),
//                Hospital = "كوبرى القبة",
//                Diagnosis = "نيحبنبحلا",
//                Recommendations = "نيحبنبحلا"
//            };
            
//            for(int i = 0; i < 10; i++)
//            {
//                Tmam tmam = new Tmam()
//                {
//                    UnitID = 4,
//                    Date = DateTime.Today.AddDays(i),
//                };
//                long tmamID = tmamService.AddTmam(tmam);
//                hospitalService.Add(
//                    new Hospital()
//                    {
//                        HospitalDetails = hospitalDetails,
//                        TmamID = tmamID,
//                        Tmam = tmam
//                    });
//                tmamList.Add(tmam);
//            }

//            long hdID = hospitalService.GetID(hospitalDetails);
//            int Expected = 10;
//            int Actual = hospitalService.GetCount(hdID);
            
//            Assert.AreEqual(Expected, Actual);
//            Expected = 1;
//            Actual = appDBContext.HospitalDetails.Where(row => row.PersonID == hospitalDetails.PersonID && row.DateFrom == hospitalDetails.DateFrom).Count();
//            Assert.AreEqual(Expected, Actual);
//            //check person status before clean
//            Expected = (int)TmamEnum.Hospital;
//            Assert.AreEqual(Expected, Actual);
            
//            //cleanup
//            hospitalService.DeleteDetails(hospitalDetails);
//            for (int i = 0; i < 10; i++)
//            {
//                tmamService.DeleteTmam(tmamList[i]);
//            }
//        }

//        [TestMethod]
//        public void TestSickLeaveService()
//        {
//            TmamService tmamService = new TmamService();
//            AppDBContext appDBContext = new AppDBContext();
//            SickLeaveService sickLeaveService = new SickLeaveService();
//            List<Tmam> tmamList = new List<Tmam>();
//            SickLeavesDetails sickLeavesDetails = new SickLeavesDetails()
//            {
//                PersonID = 233,
//                DateFrom = new DateTime(2022, 11, 28),
//                DateTo = new DateTime(2022, 12, 30),
//                Hospital = "dgagddgsadg",
//                Diagnosis = "sadgdshfdaj",
//                HospitalDate = new DateTime(2022, 11, 10)
//            };
//            for (int i = 0; i < 10; i++)
//            {
//                Tmam tmam = new Tmam()
//                {
//                    UnitID = 4,
//                    Date = DateTime.Today.AddDays(i),
//                };
//                long tmamID = tmamService.AddTmam(tmam);
//                sickLeaveService.Add(
//                    new SickLeave()
//                    {
//                        SickLeaveDetail = sickLeavesDetails,
//                        TmamID = tmamID,
//                        Tmam = tmam
//                    });
//                tmamList.Add(tmam);
//            }
//            long hdID = sickLeaveService.GetID(sickLeavesDetails);
//            int Expected = 10;
//            int Actual = sickLeaveService.GetCount(hdID);

//            Assert.AreEqual(Expected, Actual);
//            Expected = 1;
//            Actual = appDBContext.SickLeavesDetails.Where(row => row.PersonID == sickLeavesDetails.PersonID 
//                && row.DateFrom == sickLeavesDetails.DateFrom
//                && row.DateTo == sickLeavesDetails.DateTo).Count();

//            Assert.AreEqual(Expected, Actual);
//            //check person status before clean
//            Expected = (int)TmamEnum.SickLeave;
//            Actual = appDBContext.Persons.Find(sickLeavesDetails.PersonID).Status;
//            Assert.AreEqual(Expected, Actual);

//            //cleanup
//            sickLeaveService.DeleteDetails(sickLeavesDetails);
//            for (int i = 0; i < 10; i++)
//            {
//                tmamService.DeleteTmam(tmamList[i]);
//            }
//            appDBContext.Persons.Find(sickLeavesDetails.PersonID).Status = 0;
//            appDBContext.SaveChanges();
//        }

//        [TestMethod]
//        public void TestOutOfCountryService()
//        {
//            TmamService tmamService = new TmamService();
//            AppDBContext appDBContext = new AppDBContext();
//            OutOfCountryService outOfCountryService = new OutOfCountryService();
//            List<Tmam> tmamList = new List<Tmam>();
            
//            OutOfCountryDetail outOfCountryDetail = new OutOfCountryDetail()
//            {
//                PersonID = 80,
//                DateFrom = new DateTime(2022, 12, 1),
//                DateTo = new DateTime(2023, 1, 30),
//                Country = "الولايات المتحدة الأمريكية",
//                Puspose = "سياحة"
//            };

//            for (int i = 0; i < 10; i++)
//            {
//                Tmam tmam = new Tmam()
//                {
//                    UnitID = 7,
//                    Date = DateTime.Today.AddDays(i),
//                };
//                long tmamID = tmamService.AddTmam(tmam);
//                outOfCountryService.Add(
//                    new OutOfCountry()
//                    {
//                        OutOfCountryDetail = outOfCountryDetail,
//                        TmamID = tmamID,
//                        Tmam = tmam
//                    });
//                tmamList.Add(tmam);
//            }

//            long hdID = outOfCountryService.GetID(outOfCountryDetail);
//            int Expected = 10;
//            int Actual = outOfCountryService.GetCount(hdID);
//            Assert.AreEqual(Expected, Actual);

//            Expected = 1;
//            Actual = appDBContext.OutOfCountryDetails.Where(row => row.PersonID == outOfCountryDetail.PersonID
//                && row.DateFrom == outOfCountryDetail.DateFrom
//                && row.DateTo == outOfCountryDetail.DateTo).Count();

//            Assert.AreEqual(Expected, Actual);

//            //check person status before clean
//            Expected = (int)TmamEnum.OutOfCountry;
//            Actual = appDBContext.Persons.Find(outOfCountryDetail.PersonID).Status;
//            Assert.AreEqual(Expected, Actual);

//            //cleanup
//            outOfCountryService.DeleteDetails(outOfCountryDetail);
//            for (int i = 0; i < 10; i++)
//            {
//                tmamService.DeleteTmam(tmamList[i]);
//            }
//            appDBContext.Persons.Find(outOfCountryDetail.PersonID).Status = 0;
//            appDBContext.SaveChanges();
           
//        }
//        [TestMethod]
//        public void TestCampService()
//        {
//            TmamService tmamService = new TmamService();
//            AppDBContext appDBContext = new AppDBContext();
//            CampService CampService = new CampService();
//            List<Tmam> tmamList = new List<Tmam>();

//            CampDetail CampDetail = new CampDetail()
//            {
//                PersonID = 80,
//                DateFrom = new DateTime(2022, 12, 1),
//                DateTo = new DateTime(2023, 1, 30),
//                CurrentExistance = "الولايات المتحدة الأمريكية",
//                Reason = "سياحة"
//            };

//            for (int i = 0; i < 10; i++)
//            {
//                Tmam tmam = new Tmam()
//                {
//                    UnitID = 7,
//                    Date = DateTime.Today.AddDays(i),
//                };
//                long tmamID = tmamService.AddTmam(tmam);
//                CampService.Add(
//                    new Camp()
//                    {
//                        CampDetail = CampDetail,
//                        TmamID = tmamID,
//                        Tmam = tmam
//                    });
//                tmamList.Add(tmam);
//            }

//            long hdID = CampService.GetID(CampDetail);
//            int Expected = 10;
//            int Actual = CampService.GetCount(hdID);
//            Assert.AreEqual(Expected, Actual);

//            Expected = 1;
//            Actual = appDBContext.CampDetails.Where(row => row.PersonID == CampDetail.PersonID
//                && row.DateFrom == CampDetail.DateFrom
//                && row.DateTo == CampDetail.DateTo).Count();

//            Assert.AreEqual(Expected, Actual);

//            //check person status before clean
//            Expected = (int)TmamEnum.Camp;
//            Actual = appDBContext.Persons.Find(CampDetail.PersonID).Status;
//            Assert.AreEqual(Expected, Actual);

//            //cleanup
//            CampService.DeleteDetails(CampDetail);
//            for (int i = 0; i < 10; i++)
//            {
//                tmamService.DeleteTmam(tmamList[i]);
//            }
//            appDBContext.Persons.Find(CampDetail.PersonID).Status = 0;
//            appDBContext.SaveChanges();

//        }
    
//        [TestMethod]
//        public void TestConvertTimeToMilatryFormat()
//        {
//            TmamService tmamService = new TmamService();
//            DateTime time1 = new DateTime(2022,10,2,23,15,0);
//            String Expected = "2315";
//            String Acutal = tmamService.ConvertTimeToMilatryFormat(time1);
//            Assert.AreEqual(Expected, Acutal);

//            DateTime time2 = new DateTime(2022, 10, 2, 2, 43, 0);
//            String Expected2 = "0243";
//            String Acutal2 = tmamService.ConvertTimeToMilatryFormat(time2);
//            Assert.AreEqual(Expected2, Acutal2);

//            DateTime time3 = new DateTime(2022, 10, 2, 0, 46, 0);
//            String Expected3 = "0046";
//            String Acutal3 = tmamService.ConvertTimeToMilatryFormat(time3);
//            Assert.AreEqual(Expected3, Acutal3);

//            DateTime time4 = new DateTime(2022, 10, 2, 0, 5, 0);
//            String Expected4 = "0005";
//            String Acutal4 = tmamService.ConvertTimeToMilatryFormat(time4);
//            Assert.AreEqual(Expected, Acutal);
//        }
//    }
//}
