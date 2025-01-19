import pandas as pd
from joblib import load
from sklearn.preprocessing import LabelEncoder

# Step 1: Define Backend Calculations
def calculate_fields(data):
    """
    Perform backend calculations for derived fields.

    Parameters:
        data (pd.DataFrame): User input data.

    Returns:
        pd.DataFrame: Data with calculated fields added.
    """
    data['Net Cash Flow'] = data['Operating Activities - Net Cash Flow'] + data['Financing Activities - Net Cash Flow']
    data['Gross Profit/Revenue'] = data['Gross Profit (Loss)'] / data['Total Revenue']
    data['EBTI/total asset'] = data['Earnings Before Interest'] / data['Total Asset']
    data['EBTI/REV'] = data['Earnings Before Interest'] / data['Total Revenue']
    data['Total Stockholders Equity'] = data['Total Asset'] - data['Total Liabilities']
    data['Debt-to-Equity Ratio'] = (data['Total Long - Term Debt'] - data['Total Debt in Current Liabilities']) / data['Total Stockholders Equity']
    data['Working Capital Ratio'] = (data['Total Asset'] - (data['Cash'] + data['Total Inventories'])) / (data['Total Long - Term Debt'] - data['Total Debt in Current Liabilities'])
    data['Debt Service Coverage Ratio'] = data['Earnings Before Interest'] / (data['Total Long - Term Debt'] + data['Total Debt in Current Liabilities'])
    data['Gross Profit Margin'] = (data['Gross Profit (Loss)'] / data['Total Revenue']) * 100
    return data

# Step 2: Load and Preprocess Fake User Input Data
fake_user_data = pd.DataFrame({
    'Total Asset': [50000, 75000],
    'Cash': [5000, 10000],
    'Total Debt in Current Liabilities': [10000, 15000],
    'Total Long - Term Debt': [20000, 30000],
    'Earnings Before Interest': [7000, 9000],
    'Gross Profit (Loss)': [12000, 15000],
    'Total Liabilities': [40000, 60000],
    'Retained Earnings': [5000, 8000],
    'Total Stockholders Equity': [0, 0],  # Placeholder, will be recalculated
    'Total Interest and Related Expense': [2000, 3000],
    'Total Market Value': [80000, 120000],
    'Total Inventories': [10000, 15000],
    'Total Revenue': [50000, 75000],
    'Operating Activities - Net Cash Flow': [4000, 5000],
    'Financing Activities - Net Cash Flow': [2000, 3000],
})

# Perform Backend Calculations
calculated_data = calculate_fields(fake_user_data)

# Step 3: Load the Trained Model
model = load('credit_risk_model.joblib')  # Ensure this file exists

# Step 4: Select Features for Prediction
features_for_model = [
    'Total Asset', 'Cash', 'Total Debt in Current Liabilities', 
    'Total Long - Term Debt', 'Earnings Before Interest', 
    'Gross Profit (Loss)', 'Total Liabilities', 'Total Revenue', 
    'Operating Activities - Net Cash Flow', 'Financing Activities - Net Cash Flow'
]
X_for_prediction = calculated_data[features_for_model]

# Step 5: Make Predictions
predictions = model.predict(X_for_prediction)

# Decode Predictions (from numeric to category, e.g., Low/Moderate/High Risk)
label_encoder = LabelEncoder()
label_encoder.fit(['High Risk', 'Moderate Risk', 'Low Risk'])  # Adjust this based on training
decoded_predictions = label_encoder.inverse_transform(predictions)

# Step 6: Add Predictions to Data and Display Results
calculated_data['Predicted Risk'] = decoded_predictions

if __name__ == "__main__":
    print(calculated_data[['Total Asset', 'Cash', 'Predicted Risk']])