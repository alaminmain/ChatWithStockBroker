
import requests
from bs4 import BeautifulSoup
import re
import csv # Import the csv module

def get_all_instr_cds():
      url = "https://www.dsebd.org/company_listing.php"

      try:
          response = requests.get(url, timeout=10)
          response.raise_for_status() # Raise an HTTPError for bad responses (4xx or 5xx)
      except requests.exceptions.RequestException as e:
          print(f"Error fetching {url}: {e}")
          return []

      soup = BeautifulSoup(response.text, 'html.parser')

      instr_cds = []

      # Find all links that go to displayCompany.php
      # The Instr_CD is in the 'name' query parameter
      for link in soup.find_all('a', href=re.compile(r'displayCompany\.php\?name=')):
          href = link.get('href')
          match = re.search(r'name=([^&]+)', href)
          if match:
              instr_cd = match.group(1)
              if instr_cd not in instr_cds: # Avoid duplicates
                  instr_cds.append(instr_cd)

      return instr_cds

if __name__ == "__main__":
      output_file_path = 'E:\\Project\\Test Projects\\SignalRWithAngular\\instr_cds.txt'

      # Convert Windows path to WSL path for writing the file
      if output_file_path.startswith('E:\\'):
          wsl_output_file_path = '/mnt/e/' + output_file_path[3:].replace('\\', '/')
      else:
          wsl_output_file_path = output_file_path # Assume it's already a WSL path or relative

      print("Fetching all Instr_CDs from DSE company listing page...")
      all_instr_cds = get_all_instr_cds()

      if all_instr_cds:
          try:
              with open(wsl_output_file_path, 'w', encoding='utf-8') as f:
                  for instr_cd in all_instr_cds:
                      f.write(instr_cd + '\n')
              print(f"Successfully saved {len(all_instr_cds)} Instr_CDs to {wsl_output_file_path}")
          except IOError as e:
              print(f"Error writing to file {wsl_output_file_path}: {e}")
      else:
          print("No Instr_CDs found or an error occurred.")
