import matplotlib
matplotlib.use('Agg')

import pandas as pd
from joblib import load
from flask import Flask, request, jsonify
from flask_cors import CORS
import matplotlib.pyplot as plt
import io
import base64
import shap
import os

app = Flask(__name__)
CORS(app)

# Load the trained XGBoost model and LabelEncoder
model = load("Python Codes/credit_risk_model.joblib")
label_encoder = load("Python Codes/label_encoder.joblib")

# Initialize SHAP Explainer
explainer = shap.TreeExplainer(model)

# Backend calculation functions
def calculate_debt_to_equity_ratio(total_long_term_debt, total_debt_current_liabilities, total_stockholders_equity):
    return (total_long_term_debt - total_debt_current_liabilities) / total_stockholders_equity

def calculate_gross_profit_margin(gross_profit_loss, total_revenue):
    return (gross_profit_loss / total_revenue) * 100

def calculate_working_capital_ratio(total_asset, cash, total_inventories, total_long_term_debt, total_debt_current_liabilities):
    return (total_asset - (cash + total_inventories)) / (total_long_term_debt - total_debt_current_liabilities)

def calculate_debt_service_coverage_ratio(earnings_before_interest, total_long_term_debt, total_debt_current_liabilities):
    return earnings_before_interest / (total_long_term_debt + total_debt_current_liabilities)

# Create static/images directory if it doesn't exist
if not os.path.exists("wwwroot/static/images"):
    os.makedirs("wwwroot/static/images")

def generate_shap_plot(shap_values, input_data, plot_type="summary", filename="shap_plot"):
    plot_file_path = f"wwwroot/static/images/{filename}.png"
    plt.figure(figsize=(12, 8))
    
    if plot_type == "summary":
        shap.summary_plot(shap_values, input_data, show=False)
        plt.subplots_adjust(left=0.25)
    elif plot_type == "force":
        base_value = explainer.expected_value
        if isinstance(base_value, list):  # Multi-class case
            base_value = base_value[0]
            shap_values = shap_values[..., 0]
        shap.force_plot(base_value, shap_values, input_data.iloc[0], matplotlib=True)

    plt.savefig(plot_file_path, format='png', dpi=300, bbox_inches='tight')
    plt.close()
    return plot_file_path


@app.route('/predict', methods=['POST'])
def predict():
    try:
        # Parse JSON request payload
        data = request.json
        user_input = pd.DataFrame([data])

        # Perform backend calculation
        user_input["Gross Profit Margin"] = round(calculate_gross_profit_margin(
            user_input["Gross Profit (Loss)"].iloc[0],
            user_input["Total Revenue"].iloc[0]
        ), 2)
        user_input["Debt to Equity Ratio"] = round(calculate_debt_to_equity_ratio(
            user_input["Total Long-Term Debt"].iloc[0],
            user_input["Total Debt in Current Liabilities"].iloc[0],
            user_input["Total Stockholders Equity"].iloc[0]
        ), 2)
        user_input["Working Capital Ratio"] = round(calculate_working_capital_ratio(
            user_input["Total Asset"].iloc[0],
            user_input["Cash"].iloc[0],
            user_input["Total Inventories"].iloc[0],
            user_input["Total Long-Term Debt"].iloc[0],
            user_input["Total Debt in Current Liabilities"].iloc[0]
        ), 2)
        user_input["Debt Service Coverage Ratio"] = round(calculate_debt_service_coverage_ratio(
            user_input["Earnings Before Interest"].iloc[0],
            user_input["Total Long-Term Debt"].iloc[0],
            user_input["Total Debt in Current Liabilities"].iloc[0]
        ), 2)

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

        # Generate SHAP values
        shap_values = explainer.shap_values(model_input)

        # Generate and save SHAP plots
        summary_plot_path = generate_shap_plot(shap_values, model_input, plot_type="summary", filename="summary_plot")
        force_plot_path = generate_shap_plot(shap_values, model_input, plot_type="force", filename="force_plot")

        # Convert numpy types to native Python types
        return jsonify({
            "credit_risk_numerical": int(predicted_class[0]),
            "credit_risk_label": predicted_label[0],
            "calculated_ratios": {
                "Gross Profit Margin": float(user_input["Gross Profit Margin"].iloc[0]),
                "Debt to Equity Ratio": float(user_input["Debt to Equity Ratio"].iloc[0]),
                "Working Capital Ratio": float(user_input["Working Capital Ratio"].iloc[0]),
                "Debt Service Coverage Ratio": float(user_input["Debt Service Coverage Ratio"].iloc[0])
            },
            "shap_plot_paths": {
                "summary_plot": "/" + summary_plot_path,
                "force_plot": "/" + force_plot_path
            }
        })

    except Exception as e:
        return jsonify({"error": str(e)})

if __name__ == "__main__":
    app.run(host="127.0.0.1", port=5000, debug=True)