using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;


namespace App7
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }
        protected async override void OnAppearing()
        {
            base.OnAppearing();



            //Get All Persons  
            var personList = await App.SQLiteDb.GetItemsAsync();
            if (personList != null)
            {
                sPersons.ItemsSource = personList;
            }
        }

        private async void BtnAdd_Clicked(object sender, EventArgs e)
        {
            // Check if all required fields are filled
            if (!string.IsNullOrEmpty(txtPresentReading.Text) && !string.IsNullOrEmpty(txtPreviousReading.Text))
            {
                // Parse input values
                if (double.TryParse(txtPresentReading.Text, out double presentReading) && double.TryParse(txtPreviousReading.Text, out double previousReading))
                {
                    // Calculate consumption reading
                    double consumptionReading = presentReading - previousReading;
                    double Meter = double.Parse(txtMeterNo.Text);
                    // Calculate electricity charge per kilowatt hour based on consumption reading
                    double electricityChargePerKWh = 0;
                    if (consumptionReading < 72)
                        electricityChargePerKWh = 6.50;
                    else if (consumptionReading <= 150)
                        electricityChargePerKWh = 9.50;
                    else if (consumptionReading <= 300)
                        electricityChargePerKWh = 10.50;
                    else if (consumptionReading <= 400)
                        electricityChargePerKWh = 12.50;
                    else if (consumptionReading <= 500)
                        electricityChargePerKWh = 14.00;
                    else
                        electricityChargePerKWh = 16.50;

                    // Calculate electricity charge
                    double electricityCharge = consumptionReading * electricityChargePerKWh;

                    // Calculate demand charge and service charge based on type of registration
                    double demandCharge = 0;
                    double serviceCharge = 0;
                    string type;
                    switch (pickerTypeofRegistration.SelectedItem.ToString())
                    {
                        case "H":
                            type = "H";
                            demandCharge = 200;
                            serviceCharge = 50;
                            break;
                        case "B":
                            type = "B";
                            demandCharge = 400;
                            serviceCharge = 100;
                            break;


                        default:
                            await DisplayAlert("Invalid", "Invalid type of registration!", "OK");
                            return;
                    }

                    // Calculate principal amount, VAT, and amount payable
                    double principalAmount = electricityCharge + demandCharge + serviceCharge;
                    double vat = 0.05 * principalAmount;
                    double amountPayable = principalAmount + vat;

                    // Create a new ElectricityCalculation object
                    Per calculation = new Per()
                    {

                        PresentReading = presentReading,
                          PreviousReading = previousReading,
                          TypeOfRegistration = type,
                        ConsumptionReading = consumptionReading,
                        ElectricityCharge = electricityCharge,
                        DemandCharge = demandCharge,
                        ServiceCharge = serviceCharge,
                        PrincipalAmount = principalAmount,
                        Vat = vat,
                        AmountPayable = amountPayable
                    };

                    try
                    {
                        // Add the calculated data to the SQLite database
                        await App.SQLiteDb.SaveItemAsync(calculation);

                        // Retrieve the updated list of items
                        var calculationList = await App.SQLiteDb.GetItemsAsync();

                        // Assign the updated list to the table's ItemsSource
                        sPersons.ItemsSource = calculationList;

                        // Clear input fields
                        txtMeterNo.Text = string.Empty;
                        txtPresentReading.Text = string.Empty;
                        txtPreviousReading.Text = string.Empty;
                        pickerTypeofRegistration.SelectedIndex = -1;

                        // Display calculated values
                        lblmeter.Text = $"Meter No: {Meter}";
                        lblpres.Text = $"Present Reading: {presentReading}";
                        lblpres.Text = $"Present Reading: {presentReading}";
                        lblprev.Text = $"PreviousReading: {previousReading}";
                        lblConsumptionReading.Text = $"Consumption Reading: {consumptionReading}";
                        lblElectricityCharge.Text = $"Electricity Charge: {electricityCharge:C}";
                        lblDemandChargeType.Text = $"Demand Charge: {demandCharge:C}";
                        lblServiceChargeType.Text = $"Service Charge: {serviceCharge:C}";
                        lblPrincipalAmount.Text = $"Principal Amount: {principalAmount:C}";
                        lblVat.Text = $"VAT (5%): {vat:C}";
                        lblAmountPayable.Text = $"Amount Payable: {amountPayable:C}";
                    }
                    catch (Exception ex)
                    {
                        // Display an error message if an exception occurs during database operation
                        await DisplayAlert("Error", $"Failed to save calculation: {ex.Message}", "OK");
                    }
                }
                else
                {
                    await DisplayAlert("Error", "Invalid input format!", "OK");
                }
            }
            else
            {
                await DisplayAlert("Required", "Please enter all readings!", "OK");
            }
        }


















        private async void BtnUpdate_Clicked(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtMeterNo.Text))
            {
                // Fetch the existing electricity calculation from the database
                var electricityCalculation = await App.SQLiteDb.GetItemAsync(Convert.ToInt32(txtMeterNo.Text));

                if (electricityCalculation != null)
                {
                    // Store the previous values
                    double previousPresentReading = electricityCalculation.PresentReading;
                   double previousPreviousReading = electricityCalculation.PreviousReading;
                    string previousTypeOfRegistration = electricityCalculation.TypeOfRegistration;

                    // Update the properties of the existing electricity calculation object
                    electricityCalculation.PresentReading = Convert.ToInt32(txtPresentReading.Text);
                    electricityCalculation.PreviousReading = Convert.ToInt32(txtPreviousReading.Text);
                    electricityCalculation.TypeOfRegistration = pickerTypeofRegistration.SelectedItem.ToString();

                    // Recalculate consumption reading
                    double consumptionReading = electricityCalculation.PresentReading - electricityCalculation.PreviousReading;

                    // Calculate electricity charge per kilowatt hour based on consumption reading
                    double electricityChargePerKWh = 0;
                    if (consumptionReading < 72)
                        electricityChargePerKWh = 6.50;
                    else if (consumptionReading <= 150)
                        electricityChargePerKWh = 9.50;
                    else if (consumptionReading <= 300)
                        electricityChargePerKWh = 10.50;
                    else if (consumptionReading <= 400)
                        electricityChargePerKWh = 12.50;
                    else if (consumptionReading <= 500)
                        electricityChargePerKWh = 14.00;
                    else
                        electricityChargePerKWh = 16.50;

                    // Calculate electricity charge
                    double electricityCharge = consumptionReading * electricityChargePerKWh;

                    // Calculate demand charge and service charge based on type of registration
                    double demandCharge = 0;
                    double serviceCharge = 0;
                    switch (electricityCalculation.TypeOfRegistration)
                    {
                        case "H":
                            demandCharge = 200;
                            serviceCharge = 50;
                            break;
                        case "B":
                            demandCharge = 400;
                            serviceCharge = 100;
                            break;
                        default:
                            await DisplayAlert("Invalid", "Invalid type of registration!", "OK");
                            return;
                    }

                    // Calculate principal amount, VAT, and amount payable
                    double principalAmount = electricityCharge + demandCharge + serviceCharge;
                    double vat = 0.05 * principalAmount;
                    double amountPayable = principalAmount + vat;

                    // Update the calculation object with the new values
                    electricityCalculation.ConsumptionReading = consumptionReading;
                    electricityCalculation.ElectricityCharge = electricityCharge;
                    electricityCalculation.DemandCharge = demandCharge;
                    electricityCalculation.ServiceCharge = serviceCharge;
                    electricityCalculation.PrincipalAmount = principalAmount;
                    electricityCalculation.Vat = vat;
                    electricityCalculation.AmountPayable = amountPayable;

                    // Update Electricity Calculation in the database
                    await App.SQLiteDb.SaveItemAsync(electricityCalculation);

                    // Clear input fields
                    txtMeterNo.Text = string.Empty;
                    txtPresentReading.Text = string.Empty;
                    txtPreviousReading.Text = string.Empty;
                    pickerTypeofRegistration.SelectedIndex = -1;

                    // Display success message
                    await DisplayAlert("Success", "Electricity Calculation Updated Successfully", "OK");

                    // Refresh list of electricity calculations
                    var electricityCalculationList = await App.SQLiteDb.GetItemsAsync();
                    if (electricityCalculationList != null)
                    {
                        sPersons.ItemsSource = electricityCalculationList;
                    }
                }
                else
                {
                    await DisplayAlert("Error", "No electricity calculation found for the specified Meter No", "OK");
                }
            }
            else
            {
                await DisplayAlert("Required", "Please Enter Meter No", "OK");
            }
        }






        private async void BtnRead_Clicked(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtMeterNo.Text))
            {
                // Get Electricity Calculation
                var electricityCalculation = await App.SQLiteDb.GetItemAsync(Convert.ToInt32(txtMeterNo.Text));
                if (electricityCalculation != null)
                {
                    // Display the retrieved values
                    lblmeter.Text = $"Meter No: {electricityCalculation.MeterNo}"; // Add Meter No
                    lblpres.Text = $"Present Reading: {electricityCalculation.PresentReading}";
                    lblprev.Text = $"Previous Reading: {electricityCalculation.PreviousReading}";

                    lblTypeOfRegistration.Text = $"Type of Registration: {electricityCalculation.TypeOfRegistration}";
                    lblConsumptionReading.Text = $"Consumption Reading: {electricityCalculation.ConsumptionReading}";
                    lblElectricityCharge.Text = $"Electricity Charge: {electricityCalculation.ElectricityCharge:C}";
                    lblDemandChargeType.Text = $"Demand Charge: {electricityCalculation.DemandCharge:C}";
                    lblServiceChargeType.Text = $"Service Charge: {electricityCalculation.ServiceCharge:C}";
                    lblPrincipalAmount.Text = $"Principal Amount: {electricityCalculation.PrincipalAmount:C}";
                    lblAmountPayable.Text = $"Amount Payable: {electricityCalculation.AmountPayable:C}";
                    lblVat.Text = $"VAT: {electricityCalculation.Vat:C}"; // Add VAT

                    await DisplayAlert("Success", "Electricity calculation retrieved successfully", "OK");
                }
                else
                {
                    await DisplayAlert("Error", "No electricity calculation found for the specified Meter No", "OK");
                }
            }
            else
            {
                await DisplayAlert("Required", "Please Enter Meter No", "OK");
            }
        }





        private async void BtnDelete_Clicked(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtMeterNo.Text))
            {
                //Get Person  
                var person = await App.SQLiteDb.GetItemAsync(Convert.ToInt32(txtMeterNo.Text));
                if (person != null)
                {
                    //Delete Person  
                    await App.SQLiteDb.DeleteItemAsync(person);
                    txtMeterNo.Text = string.Empty;
                    await DisplayAlert("Success", "Person Deleted", "OK");

                    //Get All Persons  
                    var personList = await App.SQLiteDb.GetItemsAsync();
                    if (personList != null)
                    {
                        sPersons.ItemsSource = personList;
                    }
                }
            }
            else
            {
                await DisplayAlert("Required", "Please Enter PersonID", "OK");
            }
        }

    }

}
