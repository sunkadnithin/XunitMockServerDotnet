import os

def replace_string_in_files(directory_path, old_string, new_string):
    # Iterate over all files in the given directory
    for filename in os.listdir(directory_path):
        file_path = os.path.join(directory_path, filename)
        
        # Only process files, not directories
        if os.path.isfile(file_path):
            # Read the file content
            with open(file_path, 'r') as file:
                file_contents = file.read()

            # Replace the old_string with new_string
            new_contents = file_contents.replace(old_string, new_string)
            
            # Write the new contents back to the file if changes were made
            if new_contents != file_contents:
                with open(file_path, 'w') as file:
                    file.write(new_contents)
                print(f'Replaced in: {filename}')

def rename_files_in_directory(directory_path, old_word, new_word):
    # Iterate over all files in the given directory
    for filename in os.listdir(directory_path):
        # Check if the old_word is in the filename
        if old_word in filename:
            # Create the new filename by replacing the old_word with new_word
            new_filename = filename.replace(old_word, new_word)
            # Get the full file paths
            old_file_path = os.path.join(directory_path, filename)
            new_file_path = os.path.join(directory_path, new_filename)
            # Rename the file
            os.rename(old_file_path, new_file_path)
            print(f'Renamed: {filename} -> {new_filename}')

def get_valid_directory():
    while True:
        directory_path = input("Enter the directory path (or leave blank for current directory): ").strip()
        if not directory_path:
            directory_path = os.getcwd()
        if os.path.isdir(directory_path):
            return directory_path
        else:
            print("Invalid directory path. Please try again.")

if __name__ == '__main__':
    print("Please choose an operation:")
    print("1. Replace a word in all filenames within the directory")
    print("2. Replace a word within the contents of all files in the directory")
    choice = input("Enter your choice (1 or 2): ").strip()
    
    directory_path = get_valid_directory()
    
    switch = {
        '1': lambda: rename_files_in_directory(
            directory_path, 
            input("Enter the word to be replaced in filenames: ").strip(), 
            input("Enter the new word for filenames: ").strip()
        ),
        '2': lambda: replace_string_in_files(
            directory_path, 
            input("Enter the string to be replaced in files: ").strip(), 
            input("Enter the new string for files: ").strip()
        )
    }
    
    if choice in switch:
        switch[choice]()
    else:
        print("Invalid choice. Please run the script again and choose 1 or 2.")


'''

ReadMe.md

# File Operations Manager

This Python script allows you to perform batch operations on files within a directory:

1. **Mass Replace Word in Filenames**: Replace occurrences of a word in all filenames within a directory.
2. **Mass Replace Word in Files**: Replace occurrences of a word within the contents of all files in a directory.

## Usage

### Prerequisites

- Python 3.x installed on your system.

### Setup

1. Clone the repository or download the `file_operations_manager.py` script.

2. Navigate to the directory containing `file_operations_manager.py` using the terminal or command prompt.

### Running the Script

#### Step 1: Choose an Operation

Run the script and choose one of the following operations:

```bash
python file_operations_manager.py

'''
