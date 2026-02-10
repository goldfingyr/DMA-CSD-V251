using Newtonsoft.Json;

namespace GrabImage
{
    public partial class Form1 : Form
    {
        public string Key;
        public string Search;
        public Form1()
        {
            InitializeComponent();
        }

        private void TbKey_TextChanged(object sender, EventArgs e)
        {
            Key = TbKey.Text;
        }

        private void TbSearch_TextChanged(object sender, EventArgs e)
        {
            Search = TbSearch.Text;
        }

        private void BtSearch_Click(object sender, EventArgs e)
        {
            string searchURIfilter = "https://api.themoviedb.org/3/search/movie?include_adult=false&query=";
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, searchURIfilter + Search);
            request.Headers.Add("Accept", "application/json");
            request.Headers.Add("Authorization", "Bearer " + Key);
            var response = client.SendAsync(request).Result;
            string jsonResult = response.Content.ReadAsStringAsync().Result;
            TMDBResult Result = JsonConvert.DeserializeObject<TMDBResult>(jsonResult);
            LWMovies.Items.Clear();
            foreach (var item in Result.results)
            {
                ListViewItem myListViewItem = new ListViewItem(item.title);
                myListViewItem.SubItems.Add(item.poster_path);
                LWMovies.Items.Add(myListViewItem);
            }
        }

        private void LwSelected(object sender, EventArgs e)
        {
            // When clicking a new item, there will be a de-click first.
            if (LWMovies.SelectedItems.Count == 0)
            {
                return;
            }
            var theOne = LWMovies.SelectedItems[0];
            // Do not accept an empty path.
            if (theOne.SubItems[1].Text == "")
            {
                return;
            }
            string url = "https://image.tmdb.org/t/p/original" + theOne.SubItems[1].Text;
            try
            {
                PbSelectedImage.Load(url);
            }
            catch (Exception ex)
            {
                // MessageBox.Show("Error loading image: " + ex.Message);
            }
        }
    }

    public class Result
    {
        public string poster_path { get; set; }
        public bool adult { get; set; }
        public string overview { get; set; }
        public string release_date { get; set; }
        public List<int> genre_ids { get; set; }
        public int id { get; set; }
        public string original_title { get; set; }
        public string original_language { get; set; }
        public string title { get; set; }
        public string backdrop_path { get; set; }
        public double popularity { get; set; }
        public int vote_count { get; set; }
        public bool video { get; set; }
        public double vote_average { get; set; }
    }

    public class TMDBResult
    {
        public int page { get; set; }
        public List<Result> results { get; set; }
        public int total_pages { get; set; }
        public int total_results { get; set; }
    }
}
