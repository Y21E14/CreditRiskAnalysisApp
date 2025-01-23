import pandas as pd
import numpy as np
from xgboost import XGBClassifier
import matplotlib.pyplot as plt
import seaborn as sns
from sklearn.model_selection import StratifiedKFold, GridSearchCV  # UPDATED: Added GridSearchCV
from sklearn.preprocessing import LabelEncoder
from sklearn.metrics import accuracy_score, classification_report, confusion_matrix, cohen_kappa_score
from joblib import dump, load

# Step 1: Read the dataset
xg_df = pd.read_csv("Set B Corporate Rating - marketvalue3classes.csv")  # Load dataset
xg_df  # Display dataset

# Step 2: Column Filter Node - remove unnecessary columns (axis=1 removes columns)
xg_df = xg_df.drop(
    ['Total Revenue'],  # Exclude irrelevant columns
    axis=1,
    errors='ignore'  # Prevent errors if column doesn't exist
)

# Step 3: Correlation heatmap
numeric_data = xg_df.select_dtypes(include=[np.number])  # Select numeric columns
correlation_matrix = numeric_data.corr()  # Compute correlation matrix

plt.figure(figsize=(12, 10))  # Set figure size
sns.heatmap(correlation_matrix, annot=False, cmap='coolwarm', vmin=-1, vmax=1)
plt.title('Correlation Heatmap', fontsize=16)
plt.xticks(rotation=90)
plt.tight_layout()
plt.show()

# %%
# Define the feature matrix by dropping the target column
X_filtered = xg_df.drop(columns=['Rating level'])

# Define the target variable
y = xg_df['Rating level']

# %%
pip install xgboost

# %%
# parameter optimization loop
# Define the range for sigma
sigma_values = np.arange(0.3, 0.6 + 0.1, 0.1)  # Start, Stop (inclusive), Step
print("Sigma values to evaluate:", sigma_values)

# %%
# Store the results
results = []

for sigma in sigma_values:
    print(f"Training with sigma = {sigma}")
    
    # Define the model with sigma as the hyperparameter (example usage in 'subsample')
    model = XGBClassifier(subsample=sigma, random_state=42)
    
    # Perform cross-validation (using StratifiedKFold from your earlier setup)
    fold_accuracies = []
    for train_index, test_index in skf.split(X_filtered, y_encoded):
        X_train, X_test = X_filtered.iloc[train_index], X_filtered.iloc[test_index]
        y_train, y_test = y_encoded[train_index], y_encoded[test_index]
        
        # Train the model
        model.fit(X_train, y_train)
        
        # Make predictions
        y_pred = model.predict(X_test)
        
        # Calculate accuracy
        accuracy = accuracy_score(y_test, y_pred)
        fold_accuracies.append(accuracy)
    
    # Store the average accuracy for this sigma
    avg_accuracy = np.mean(fold_accuracies)
    results.append((sigma, avg_accuracy))
    print(f"Sigma = {sigma}, Average Accuracy = {avg_accuracy:.4f}")

# Display the best sigma value
best_sigma, best_accuracy = max(results, key=lambda x: x[1])
print(f"\nBest Sigma: {best_sigma}, Best Average Accuracy: {best_accuracy:.4f}")

# %%
# x-partitioner and xgboost tree learner
from sklearn.model_selection import StratifiedKFold
from sklearn.metrics import accuracy_score
from xgboost import XGBClassifier
from sklearn.preprocessing import LabelEncoder
import numpy as np

# Step 1: Encode the target labels
label_encoder = LabelEncoder()
y_encoded = label_encoder.fit_transform(y)  # Encode 'High Risk', 'Low Risk', etc. to 0, 1, 2

# Number of validation folds
num_folds = 38  # Adjust based on your needs
skf = StratifiedKFold(n_splits=num_folds, shuffle=True, random_state=42)

all_y_test = []
all_y_test_pred = []

# Perform cross-validation
for train_index, test_index in skf.split(X_filtered, y_encoded):
    X_train, X_test = X_filtered.iloc[train_index], X_filtered.iloc[test_index]
    y_train, y_test = y_encoded[train_index], y_encoded[test_index]

    # Predict using the best model
    y_test_pred = best_model.predict(X_test)
    all_y_test.extend(y_test)
    all_y_test_pred.extend(y_test_pred)

# Confusion Matrix
conf_matrix = confusion_matrix(all_y_test, all_y_test_pred)
print("\nConfusion Matrix:")
print(conf_matrix)

# Visualize Confusion Matrix
plt.figure(figsize=(8, 6))
sns.heatmap(conf_matrix, annot=True, fmt='d', cmap='coolwarm',
            xticklabels=label_encoder.classes_, yticklabels=label_encoder.classes_)
plt.title("Confusion Matrix")
plt.xlabel("Predicted")
plt.ylabel("True")
plt.tight_layout()
plt.show()

# Classification Report
print("\nClassification Report:")
print(classification_report(all_y_test, all_y_test_pred, target_names=label_encoder.classes_))

# Overall Accuracy
overall_accuracy = accuracy_score(all_y_test, all_y_test_pred)
print(f"\nOverall Accuracy: {overall_accuracy:.2%}")

# Cohen's Kappa
kappa = cohen_kappa_score(all_y_test, all_y_test_pred)
print(f"\nCohen's Kappa: {kappa:.3f}")
