using ElecWarSystem.Data;
using ElecWarSystem.Models;
using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace ElecWarSystem.Serivces
{
    public class UserService
    {
        private static UserService instance = null;
        public AppDBContext dBContext { get; set; }

        //lockObj using to make the instance is thread Safe
        private static object lockObj = new object();
        public UserService()
        {
            dBContext = new AppDBContext();
        }
        //we using SingleTon Design Pattern
        public static UserService GetInstance()
        {
            lock (lockObj)
            {
                if (instance == null)
                    instance = new UserService();

                return instance;
            }
        }
        //hashing the password using md5
        public static String getPasswordHash(String password)
        {
            //Hash Password using MD5 Hashing Algorithm
            MD5 md5 = MD5.Create();
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
            byte[] passwordHash = md5.ComputeHash(passwordBytes);
            String passwordHashString = BitConverter.ToString(passwordHash).Replace("-", string.Empty);
            return passwordHashString;
        }
        //Check if the user is Found or Not
        public bool UserNameIsExist(String userName, int unitID)
        {
            bool result = false;
            User user = dBContext.Users.FirstOrDefault(row => String.Compare(row.UserName, userName) == 0 || row.UnitID == unitID);
            result = (user != null);
            return result;
        }

        //Get List Of all Users
        public List<Unit> getAllUsers()
        {
            List<Unit> users = dBContext.Units.ToList();
            return users;
        }
        public List<int> getAllUnitIDs()
        {
            List<int> userIds = dBContext.Units.OrderBy(row => row.Order).Where(row => row.Order > 0).Select(row => row.ID).ToList();
            return userIds;
        }
        //Create New Account and store it in database
        public int CreateNewUser(User user, String confirmPassword)
        {
            int result = 0;
            if (String.Compare(user.Password, confirmPassword) == 0)
            {
                try
                {
                    user.Password = getPasswordHash(user.Password);
                    dBContext.Users.Add(user);
                    dBContext.SaveChanges();
                    result = user.ID;
                }
                catch (Exception ex)
                {
                    result = -2;
                }

            }
            else
            {
                result = -1;
            }
            return result;
        }
        //Delete User
        public bool DeleteUser(Unit user)
        {
            try
            {
                dBContext.Units.Remove(user);
                dBContext.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        //3FEDB74630C6C5D3E2C7813AD3B42C78
        //3FEDB74630C6C5D3E2C7813AD3B42C78
        //Edit User
        public User GetUser(String userName, String Password)
        {
            User user = dBContext.Users.Include("Unit").FirstOrDefault(row => row.UserName == userName && row.Password.CompareTo(Password) == 0);
            return user;
        }
        public Unit GetUnit(int id)
        {
            return dBContext.Units.Include("SmallUnits").FirstOrDefault(row => row.ID == id);
        }
        public String GetUnitName(int id)
        {
            String unitName = GetUnit(id).UnitName;
            return unitName;
        }

        public User AuthenticateUser(String userName, String password)
        {
            password = getPasswordHash(password);
            User user = GetUser(userName, password);
            return user;
        }
        //public void ActivateUser(String userName, bool active)
        //{
        //    User user = GetUnit(userName);
        //    user.isActive = active;
        //    dBContext.SaveChanges();
        //}
    }
}