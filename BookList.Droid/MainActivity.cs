using Android.App;
using Android.Widget;
using Android.OS;
using SQLite;
using System.IO;
using System.Collections.Generic;

namespace BookList.Droid
{
    [Activity(Label = "BookList.Droid", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        EditText txtTitle;
        EditText txtISBN;
        ListView tblBooks;
        Button btnAdd;
        List<string> bookTitles;
        string filePath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), "BookList.db3");

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.Main);
            txtTitle = FindViewById<EditText>(Resource.Id.txtTitle);
            txtISBN = FindViewById<EditText>(Resource.Id.txtISBN);
            tblBooks = FindViewById<ListView>(Resource.Id.tblBooks);
            btnAdd = FindViewById<Button>(Resource.Id.btnAdd);
            btnAdd.Click += BtnAdd_Click;

            PopulateListView();

            try
            {
                var db = new SQLiteConnection(filePath);
                db.CreateTable<Book>();
            }
            catch (IOException ex)
            {
                var reason = string.Format("Failed to create Table - reason { 0}", ex.Message);
                Toast.MakeText(this, reason, ToastLength.Long).Show();
            }
            tblBooks.ItemClick += TblBooks_ItemClick;
        }

        private void TblBooks_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            var viewItem = bookTitles[e.Position];
            var dialog = new AlertDialog.Builder(this);
            dialog.SetMessage(viewItem.ToString());
            dialog.SetNeutralButton("OK", delegate { });
            dialog.Show();
        }

        void BtnAdd_Click(object sender, System.EventArgs e)
        {
            string alertTitle, alertMessage;
            if (!string.IsNullOrEmpty(txtTitle.Text))
            {
                var newBook = new Book { BookTitle = txtTitle.Text, ISBN = txtISBN.Text };
                var db = new SQLiteConnection(filePath);
                db.Insert(newBook);
                alertTitle = "Success";
                alertMessage = string.Format("Book ID: {0} with Title: {1} has been successfully added!", newBook.BookId, newBook.BookTitle);
            }
            else
            {
                alertTitle = "Failed";
                alertMessage = "Enter a vail Book Title";
            }

            AlertDialog.Builder alert = new AlertDialog.Builder(this);
            alert.SetTitle(alertTitle);
            alert.SetMessage(alertMessage);
            alert.SetPositiveButton("OK", (senderAlert, args) => { Toast.MakeText(this, "Continue!", ToastLength.Short).Show(); });
            alert.SetNegativeButton("Cancel", (senderAlert, args) => { Toast.MakeText(this, "Cancelled!", ToastLength.Short).Show(); });
            Dialog dialog = alert.Create();
            dialog.Show();
            txtTitle.Text = "";
            txtISBN.Text = "";
            PopulateListView();


        }
        private void PopulateListView()
        {
            var db = new SQLiteConnection(filePath);
            var bookList = db.Table<Book>();
            bookTitles = new List<string>();
            foreach (var book in bookList)
            {
                bookTitles.Add(book.BookTitle);
            }
            tblBooks.Adapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleListItem1, bookTitles.ToArray());
        }
    }
}
