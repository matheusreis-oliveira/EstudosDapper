using System;
using System.Data;
using System.Linq;
using EstudosDapper.Models;
using Dapper;

using Microsoft.Data.SqlClient;
using System.Collections.Generic;

namespace EstudosDapper
{
    class Program
    {
        static void Main(string[] args)
        {
            const string connectString = "Server=MATHEUS;Database=Estudos;User ID=sa;Password=123456";

            using (var connection = new SqlConnection(connectString))
            {
                //CreateCategory(connection);
                //CreateManyCategories(connection);
                //UpdateCategory(connection);
                //DeleteCategory(connection);
                //ListCategories(connection);
                //GetCategory(connection);
                //ExecuteProcedure(connection);
                //ExecuteReadProcedure(connection);
                //ExecuteScalar(connection);
                //ReadView(connection);
                //OneToOne(connection);
                //OneToMany(connection);
                //QueryMutiple(connection);
                //SelectIn(connection);
                //Like(connection, "backend");
                //Transaction(connection);
            }
        }
        static void ListCategories(SqlConnection connection)
        {
            var categories = connection.Query<CategoryModel>("SELECT ctg.Id, ctg.Title, ctg.Url FROM Category ctg");

            foreach (var item in categories)
            {
                Console.WriteLine($"{item.Id} - {item.Title} - exemple.com.br/{item.Url}");
            }
        }
        static void GetCategory(SqlConnection connection)
        {
            var category = connection.QueryFirstOrDefault<CategoryModel>(
                    "SELECT TOP 1 Id, Title FROM Category WHERE Id=@id",
                    new
                    {
                        id = "af3407aa-11ae-4621-a2ef-2028b85507c4"
                    });

            Console.WriteLine($"{category.Id} - {category.Title}");
        }
        static void CreateCategory(SqlConnection connection)
        {
            var category = new CategoryModel();
            category.Id = Guid.NewGuid();
            category.Title = "Amazon AWS";
            category.Url = "amazon";
            category.Summary = "AWS Cloud";
            category.Order = 8;
            category.Description = "Categoria destianada a serviços do AWS";
            category.Featured = false;

            // sql injection = nunca concatenar string
            // sql parameter = ao usar o mesmo nome (camelCase) nao é necessario referenciar o parametro
            var insertSql = @"INSERT INTO Category VALUES(@Id, @Title, @Url, @Summary, @Order, @Description, @Featured)";

            var rows = connection.Execute(insertSql, new
            {
                category.Id,
                category.Title,
                category.Url,
                category.Summary,
                category.Order,
                category.Description,
                category.Featured
            });

            Console.WriteLine($"{rows} linhas inseridas");
        }
        static void UpdateCategory(SqlConnection connection)
        {

            var updateQuery = "UPDATE Category SET Title=@title WHERE Id = @id";
            var rows = connection.Execute(updateQuery, new
            {
                id = new Guid("af3407aa-11ae-4621-a2ef-2028b85507c4"),
                title = "Frontend"
            });

            Console.WriteLine($"{rows} registros atualizados");
        }
        static void DeleteCategory(SqlConnection connection)
        {
            var deleteQuery = "DELETE Category WHERE id=@id";
            var rows = connection.Execute(deleteQuery, new
            {
                id = new Guid("ea8059a2-e679-4e74-99b5-e4f0b310fe6f"),
            });

            Console.WriteLine($"{rows} registros excluídos");
        }
        static void CreateManyCategories(SqlConnection connection)
        {
            var category = new CategoryModel();
            category.Id = Guid.NewGuid();
            category.Title = "Google Ads";
            category.Url = "google-ads";
            category.Summary = "Google Ads";
            category.Order = 9;
            category.Description = "Categoria destianada a serviços do Google Adons";
            category.Featured = false;

            var category2 = new CategoryModel();
            category2.Id = Guid.NewGuid();
            category2.Title = "Categoria Nova";
            category2.Url = "categoria-nova";
            category2.Summary = "Categoria";
            category2.Order = 10;
            category2.Description = "Categoria Nova";
            category2.Featured = true;

            // sql injection = nunca concatenar string
            // sql parameter = ao usar o mesmo nome (camelCase) nao é necessario referenciar o parametro
            var insertSql = @"INSERT INTO Category VALUES(@Id, @Title, @Url, @Summary, @Order, @Description, @Featured)";

            var rows = connection.Execute(insertSql, new[]{
                new{
                category.Id,
                category.Title,
                category.Url,
                category.Summary,
                category.Order,
                category.Description,
                category.Featured
            },
            new{
                category2.Id,
                category2.Title,
                category2.Url,
                category2.Summary,
                category2.Order,
                category2.Description,
                category2.Featured
            }
            });

            Console.WriteLine($"{rows} linhas inseridas");
        }
        static void ExecuteProcedure(SqlConnection connection)
        {
            var procedure = "spDeleteStudent";
            var pars = new { StudentId = "b5ee3c03-287f-49da-b6c1-5a0afd071786" };
            var effectedRows = connection.Execute(procedure, pars, commandType: CommandType.StoredProcedure);

            Console.WriteLine($"{effectedRows} linhas executadas");
        }
        static void ExecuteReadProcedure(SqlConnection connection)
        {
            var procedure = "spGetCoursesByCategory";
            var pars = new { CategoryId = "09ce0b7b-cfca-497b-92c0-3290ad9d5142" };
            var courses = connection.Query(procedure, pars, commandType: CommandType.StoredProcedure);

            foreach (var item in courses)
            {
                Console.WriteLine($"{item.Id} - {item.Title} - exemple.com.br/{item.Url}");
            }
        }
        static void ExecuteScalar(SqlConnection connection)
        {
            var category = new CategoryModel();
            category.Title = "Categoria Scalar";
            category.Url = "categoria-scalar";
            category.Summary = "Categorai Scalar";
            category.Order = 11;
            category.Description = "Categoria destianada ao uso do Scalar";
            category.Featured = false;

            var insertSql = @"INSERT INTO Category OUTPUT inserted.Id VALUES(NEWID(), @Title, @Url, @Summary, @Order, @Description, @Featured)";

            var id = connection.ExecuteScalar<Guid>(insertSql, new
            {
                category.Title,
                category.Url,
                category.Summary,
                category.Order,
                category.Description,
                category.Featured
            });

            Console.WriteLine($"o Id da categoria inserida foi: {id}");

        }
        static void ReadView(SqlConnection connection)
        {
            var sql = "SELECT * FROM vwCourses";
            var courses = connection.Query<CategoryModel>(sql);

            foreach (var item in courses)
            {
                Console.WriteLine($"{item.Id} - {item.Title} - exemple.com.br/{item.Url}");
            }
        }
        static void OneToOne(SqlConnection connection)
        {
            var sql = @"SELECT * FROM CareerItem carit
                            JOIN Course crs ON carit.CourseId = crs.Id";

            var items = connection.Query<CareerItemModel, CourseModel, CareerItemModel>(
            sql,
            (careerItemModel, courseModel) =>
            {
                careerItemModel.Course = courseModel;
                return careerItemModel;
            }, splitOn: "Id");

            foreach (var item in items)
            {
                Console.WriteLine($"Curso: {item.Course.Title} - {item.Title}");
            }
        }
        static void OneToMany(SqlConnection connection)
        {
            var sql = @"SELECT car.Id, car.Title, carit.CareerId, carit.Title FROM Career car
                            JOIN CareerItem carit ON carit.CareerId = car.Id
                        ORDER BY car.Title";

            var careers = new List<CareerModel>();
            var items = connection.Query<CareerModel, CareerItemModel, CareerModel>(
            sql,
            (career, careerItem) =>
            {
                var car = careers.Where(c => c.Id == career.Id).FirstOrDefault();

                if (car == null)
                {
                    car = career; //faço car receber career
                    car.Items.Add(careerItem); //adicionando um novo item a careerItem
                    careers.Add(car); //adicionando a car na lista de careers
                }
                else
                {
                    car.Items.Add(careerItem); // se ja tem, apenas adiciono um novo item no careerItem
                }

                return career;
            }, splitOn: "CareerId");

            foreach (var career in careers)
            {
                Console.WriteLine($"{career.Title}");
                foreach (var item in career.Items)
                {
                    Console.WriteLine($"- {item.Title}");
                }
            }
        }
        static void QueryMutiple(SqlConnection connection)
        {
            var sql = "SELECT * FROM Category; SELECT * FROM Course";

            using (var multi = connection.QueryMultiple(sql))
            {
                var categories = multi.Read<CategoryModel>();
                var courses = multi.Read<CourseModel>();

                foreach (var item in categories)
                {
                    Console.WriteLine(item.Title);
                }
                foreach (var item in courses)
                {
                    Console.WriteLine(item.Title);
                }
            }
        }
        static void SelectIn(SqlConnection connection)
        {
            var sql = @"SELECT * FROM Career WHERE ID IN @Id";

            var items = connection.Query<CareerModel>(sql, new
            {
                Id = new[] { "4327ac7e-963b-4893-9f31-9a3b28a4e72b", "e6730d1c-6870-4df3-ae68-438624e04c72" }
            });

            foreach (var item in items)
            {
                Console.WriteLine(item.Title);
            }
        }
        static void Like(SqlConnection connection, string term)
        {
            var sql = @"SELECT * FROM Course WHERE Title LIKE @expression";

            var items = connection.Query<CourseModel>(sql, new
            {
                expression = $"%{term}%"
            });

            foreach (var item in items)
            {
                Console.WriteLine(item.Title);
            }
        }
        static void Transaction(SqlConnection connection)
        {
            //duplicação do createCateory para utilizar dentro de uma transaction
            var category = new CategoryModel();
            category.Id = Guid.NewGuid();
            category.Title = "Amazon AWS";
            category.Url = "amazon";
            category.Summary = "AWS Cloud";
            category.Order = 8;
            category.Description = "Categoria destianada a serviços do AWS";
            category.Featured = false;

            var insertSql = @"INSERT INTO Category VALUES(@Id, @Title, @Url, @Summary, @Order, @Description, @Featured)";

            connection.Open(); //abrindo a conexão para nao acusar que a conexão está fechada
            using (var transaction = connection.BeginTransaction())
            {
                var rows = connection.Execute(insertSql, new
                {
                    category.Id,
                    category.Title,
                    category.Url,
                    category.Summary,
                    category.Order,
                    category.Description,
                    category.Featured
                }, transaction);

                //transaction.Commit();
                transaction.Rollback();
                Console.WriteLine($"{rows} linhas inseridas");
            }
            connection.Close();
        }
    }
}