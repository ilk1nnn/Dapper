using Dapper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfApp1.Entities;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public List<Player> GetAll()
        {
            List<Player> players = new List<Player>();
            var conn_string = ConfigurationManager.ConnectionStrings["MyConnString"].ConnectionString;
            using (var conn = new SqlConnection(conn_string))
            {
                players = conn.Query<Player>("SELECT * FROM Players").ToList();
            }
            return players;
        }

        public Player GetById(int id)
        {
            var conn_string = ConfigurationManager.ConnectionStrings["MyConnString"].ConnectionString;
            using (var conn = new SqlConnection(conn_string))
            {
                var player = conn.QueryFirstOrDefault<Player>("SELECT * FROM Players WHERE Id = @Pid",
                    new { Pid = id });

                return player;
            }
        }


        public void Update(Player player)
        {
            var conn_string = ConfigurationManager.ConnectionStrings["MyConnString"].ConnectionString;
            using (var conn = new SqlConnection(conn_string))
            {
                conn.Execute(@"UPDATE Players
                               SET [Name] = @name,[Score] = @score,[IsStar] = @isStar
                               WHERE Id = @Pid", new { name = player.Name, score = player.Score, isStar = player.IsStar, Pid = player.Id }
                               );

            }
        }

        public void AddPlayer(Player player)
        {
            var conn_string = ConfigurationManager.ConnectionStrings["MyConnString"].ConnectionString;
            using (var conn = new SqlConnection(conn_string))
            {
                conn.Execute(@"INSERT INTO Players([Name],[Score],[IsStar])
                               VALUES(@name,@score,@isStar)",
                               new { name = player.Name, score = player.Score, isStar = player.IsStar });
            }
        }

        public void DeletePlayer(int id)
        {
            var conn_string = ConfigurationManager.ConnectionStrings["MyConnString"].ConnectionString;
            using (var conn = new SqlConnection(conn_string))
            {
                conn.Execute("DELETE FROM Players WHERE Id = @id", new { id = id });
                MessageBox.Show("Deleted Successfully!");
            }
        }

        public MainWindow()
        {
            InitializeComponent();

            //var player = GetById(2);
            //player.Name = "Rafiq";
            //player.Score = 20;

            //Update(player);


            //var player = new Player
            //{
            //    Name = "Akif",
            //    Score = 25,
            //    IsStar = true
            //};
            //AddPlayer(player);
            var players = GetAll();
            myDataGrid.ItemsSource = players;
            CallStoreProcedure(0);

            //var player = GetById(2);
            //myDataGrid.ItemsSource = new List<Player> {player};




        }

        public void CallStoreProcedure(float score)
        {
            var conn_string = ConfigurationManager.ConnectionStrings["MyConnString"].ConnectionString;
            using (var conn = new SqlConnection(conn_string))
            {
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@PScore", score, System.Data.DbType.Double);
                var data = conn.Query<Player>("sp_ShowGreaterThan",parameters,commandType:System.Data.CommandType.StoredProcedure);
                myDataGrid.ItemsSource = data;

            }
        }




        public bool HasAlreadyDeleted { get; set; }
        

        private void myDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var player = myDataGrid.SelectedItem as Player;
            if(!HasAlreadyDeleted)
            {
                var result = MessageBox.Show($"Are you sure to delete {player?.Name}", "Info", MessageBoxButton.YesNo);
                HasAlreadyDeleted = true;
                if (result == MessageBoxResult.Yes)
                {

                    DeletePlayer(player.Id);
                    var players = GetAll();
                    myDataGrid.ItemsSource = players;
                    HasAlreadyDeleted = false;
                }
                else
                {
                    Name.Text = player.Name;
                    Score.Value = player.Score;
                    IsStar.IsChecked = player.IsStar;
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var player = myDataGrid.SelectedItem as Player;
            var newPlayer = new Player
            {
                Id = player.Id,
                Name = Name.Text,
                IsStar = IsStar.IsChecked.Value,
                Score = Score.Value
            };
            Update(newPlayer);
            var players = GetAll();
            myDataGrid.ItemsSource = players;
            HasAlreadyDeleted = false;
        }
    }
}
