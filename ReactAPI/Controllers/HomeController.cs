using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web.Http;

namespace ReactAPI.Controllers
{
    public class HomeController : ApiController
    {
        public static string dbConn = "Data Source=localhost; user id=appuser; password=app_password; initial catalog=app;";

        [HttpPost]
        public int Register([FromBody] Member member)
        {
            using (IDbConnection conn = new SqlConnection(dbConn))
            {
                conn.Open();

                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "INSERT into person (FirstName, LastName) values ('" + member.FirstName + "', '" + member.LastName + "'); SELECT SCOPE_IDENTITY();";
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }

        [HttpPost]
        public int EditMember([FromBody] Member member)
        {            
            using (IDbConnection conn = new SqlConnection(dbConn))
            {
                conn.Open();

                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = string.Format("UPDATE Person SET FirstName = '{0}', LastName = '{1}' WHERE Id = {2}",
                        member.FirstName, member.LastName, member.Id);
                    return cmd.ExecuteNonQuery();                   
                }
            }            
        }

        [HttpGet]
        public List<Member> GetMembers()
        {
            var members = new List<Member>();

            using (IDbConnection conn = new SqlConnection(dbConn))
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "SELECT * FROM Person";
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            members.Add(new Member()
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                FirstName = Convert.ToString(reader["FirstName"]),
                                LastName = Convert.ToString(reader["LastName"])
                            });
                        }
                    }
                }
            }

            return members;
        }

        [HttpGet]
        public Member GetMemberById(int id)
        {
            Member member = null;

            using (var conn = new SqlConnection(dbConn))
            {
                conn.Open();

                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = string.Format("SELECT * FROM Person WHERE ID = {0}", id);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if(reader.HasRows)
                        {
                            reader.Read();
                            member = new Member()
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                FirstName = Convert.ToString(reader["FirstName"]),
                                LastName = Convert.ToString(reader["LastName"])
                            };
                        }
                    }
                }
            }
            
            return member;
        }

        [HttpPost]
        public void DeleteMember(Member member)
        {
            if (member == null || member.Id == 0)
                return;

            using (IDbConnection conn = new SqlConnection(dbConn))
            {
                conn.Open();

                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "DELETE FROM person WHERE Id = " + member.Id;
                    cmd.ExecuteScalar();
                }
            }
        }
    }

    public class Member
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
