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

# Step 4: Define features (X) and target variable (y)
X_filtered = xg_df.drop(columns=['Rating level'])  # Features
y = xg_df['Rating level']  # Target

# Step 5: Encode the target labels
label_encoder = LabelEncoder()
y_encoded = label_encoder.fit_transform(y)

# Save the LabelEncoder for reuse
dump(label_encoder, 'joblib/label_encoder.joblib')  # ADDED: Save LabelEncoder in the 'joblib' folder

# Step 6: Parameter Optimization with GridSearchCV - UPDATED
# Define parameter grid for optimization
param_grid = {
    'max_depth': [3, 5, 7],          # Tree depth
    'learning_rate': [0.1, 0.2, 0.3],  # Learning rate (eta)
    'n_estimators': [50, 100, 150],   # Number of trees
    'subsample': [0.8, 1.0],          # Subsample ratio
    'colsample_bytree': [0.8, 1.0]    # Column subsample ratio
}

# Initialize GridSearchCV
grid_search = GridSearchCV(
    estimator=XGBClassifier(random_state=42),  # Base model
    param_grid=param_grid,
    cv=5,  # 5-fold cross-validation
    scoring='accuracy',  # Optimize accuracy
    verbose=1,  # Display progress
    n_jobs=-1  # Use all available CPU cores
)

print("Running Grid Search...")  # ADDED: Inform user of progress
grid_search.fit(X_filtered, y_encoded)  # Run GridSearchCV

# Display best parameters and score
print("\nBest Parameters:", grid_search.best_params_)  # ADDED: Show best parameters
print("Best Score (Accuracy):", grid_search.best_score_)  # ADDED: Show best accuracy

# Step 7: Train Final Model with Best Parameters - UPDATED
# Retrieve the best parameters
best_params = grid_search.best_params_

# Initialize model with best parameters
best_model = XGBClassifier(**best_params, random_state=42)
best_model.fit(X_filtered, y_encoded)  # Train final model

# Save the trained model
dump(best_model, 'joblib/credit_risk_model.joblib')  # UPDATED: Save the model in the 'joblib' folder

# Step 8: Evaluate the Model
# Cross-validation with the best model
num_folds = 5
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
