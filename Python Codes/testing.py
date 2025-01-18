import pandas as pd
import numpy as np
from sklearn.model_selection import StratifiedKFold
from sklearn.preprocessing import LabelEncoder
from sklearn.metrics import accuracy_score, classification_report, confusion_matrix
from xgboost import XGBClassifier

# Step 1: Load the dataset
file_path = "fake_credit_rating_data.csv"
data = pd.read_csv(file_path)

# Step 2: Preprocess the data
# Define features (X) and target (y)
X = data.drop(columns=["Rating level"])
y = data["Rating level"]

# Encode the target labels
y_encoded = LabelEncoder().fit_transform(y)

# Step 3: Set up cross-validation
num_folds = 5
skf = StratifiedKFold(n_splits=num_folds, shuffle=True, random_state=42)

# Store results
train_accuracies = []
test_accuracies = []
all_y_test = []
all_y_test_pred = []

# Step 4: Train and test the model using cross-validation
for train_idx, test_idx in skf.split(X, y_encoded):
    # Split data into training and testing sets
    X_train, X_test = X.iloc[train_idx], X.iloc[test_idx]
    y_train, y_test = y_encoded[train_idx], y_encoded[test_idx]

    # Define the XGBoost model
    model = XGBClassifier(
        max_depth=3,
        learning_rate=0.1,
        n_estimators=100,
        random_state=42,
        objective='multi:softmax'
    )

    # Train the model
    model.fit(X_train, y_train)

    # Evaluate on training data
    y_train_pred = model.predict(X_train)
    train_accuracies.append(accuracy_score(y_train, y_train_pred))

    # Evaluate on testing data
    y_test_pred = model.predict(X_test)
    test_accuracies.append(accuracy_score(y_test, y_test_pred))

    # Store predictions for aggregated metrics
    all_y_test.extend(y_test)
    all_y_test_pred.extend(y_test_pred)

# Step 5: Calculate overall performance metrics
print("Training Accuracy per Fold:", train_accuracies)
print("Testing Accuracy per Fold:", test_accuracies)
print(f"Average Training Accuracy: {np.mean(train_accuracies):.2f}")
print(f"Average Testing Accuracy: {np.mean(test_accuracies):.2f}")

# Classification report
print("\nClassification Report:")
print(classification_report(all_y_test, all_y_test_pred, target_names=np.unique(y)))

# Confusion matrix
print("\nConfusion Matrix:")
conf_matrix = confusion_matrix(all_y_test, all_y_test_pred)
print(conf_matrix)

# Display confusion matrix percentages
conf_matrix_percentages = conf_matrix.astype('float') / conf_matrix.sum(axis=1)[:, np.newaxis] * 100
print("\nConfusion Matrix (Percentages):")
print(np.round(conf_matrix_percentages, 2))
