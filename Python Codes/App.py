import pandas as pd
from joblib import load
from flask import Flask, request, jsonify
from flask_cors import CORS

app = Flask(__name__)
CORS(app)

# Load the trained XGBoost model and LabelEncoder
model = load("Python Codes/credit_risk_model.joblib")
label_encoder = load("Python Codes/label_encoder.joblib")

# Backend calculation functions
def calculate_debt_to_equity_ratio(total_long_term_debt, total_debt_current_liabilities, total_stockholders_equity):
    return (total_long_term_debt - total_debt_current_liabilities) / total_stockholders_equity

def calculate_net_cash_flow(operating_activities, financing_activities):
    return operating_activities + financing_activities

def calculate_gross_profit_margin(gross_profit_loss, total_revenue):
    return (gross_profit_loss / total_revenue) * 100

def calculate_working_capital_ratio(total_asset, cash, total_inventories, total_long_term_debt, total_debt_current_liabilities):
    return (total_asset - (cash + total_inventories)) / (total_long_term_debt - total_debt_current_liabilities)

def calculate_debt_service_coverage_ratio(earnings_before_interest, total_long_term_debt, total_debt_current_liabilities):
    return earnings_before_interest / (total_long_term_debt + total_debt_current_liabilities)

@app.route('/predict', methods=['POST'])
def predict():
    try:
        # Parse JSON request payload
        data = request.json
        user_input = pd.DataFrame([data])

        # Perform backend calculations
        user_input["Net Cash Flow"] = calculate_net_cash_flow(
            user_input["Operating Activities - Net Cash Flow"].iloc[0],
            user_input["Financing Activities - Net Cash Flow"].iloc[0]
        )
        user_input["Gross Profit Margin"] = calculate_gross_profit_margin(
            user_input["Gross Profit (Loss)"].iloc[0],
            user_input["Total Revenue"].iloc[0]
        )
        user_input["Debt to Equity Ratio"] = calculate_debt_to_equity_ratio(
            user_input["Total Long-Term Debt"].iloc[0],
            user_input["Total Debt in Current Liabilities"].iloc[0],
            user_input["Total Stockholders Equity"].iloc[0]
        )
        user_input["Working Capital Ratio"] = calculate_working_capital_ratio(
            user_input["Total Asset"].iloc[0],
            user_input["Cash"].iloc[0],
            user_input["Total Inventories"].iloc[0],
            user_input["Total Long-Term Debt"].iloc[0],
            user_input["Total Debt in Current Liabilities"].iloc[0]
        )
        user_input["Debt Service Coverage Ratio"] = calculate_debt_service_coverage_ratio(
            user_input["Earnings Before Interest"].iloc[0],
            user_input["Total Long-Term Debt"].iloc[0],
            user_input["Total Debt in Current Liabilities"].iloc[0]
        )

        # Ensure all required features are included
        feature_names = model.get_booster().feature_names
        for feature in feature_names:
            if feature not in user_input.columns:
                user_input[feature] = 0

        # Align input features with the model
        model_input = user_input[feature_names]

        # Predict the risk using the model directly
        predicted_class = model.predict(model_input)
        predicted_label = label_encoder.inverse_transform([int(predicted_class[0])])

        # Convert numpy types to native Python types
        return jsonify({
            "credit_risk_numerical": int(predicted_class[0]),
            "credit_risk_label": predicted_label[0],
            "calculated_ratios": {
                "Net Cash Flow": float(user_input["Net Cash Flow"].iloc[0]),
                "Gross Profit Margin": float(user_input["Gross Profit Margin"].iloc[0]),
                "Debt to Equity Ratio": float(user_input["Debt to Equity Ratio"].iloc[0]),
                "Working Capital Ratio": float(user_input["Working Capital Ratio"].iloc[0]),
                "Debt Service Coverage Ratio": float(user_input["Debt Service Coverage Ratio"].iloc[0])
            }
        })

    except Exception as e:
        return jsonify({"error": str(e)})


if __name__ == "__main__":
    app.run(host="127.0.0.1", port=5000, debug=True)
