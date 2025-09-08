import requests
from bs4 import BeautifulSoup
import re
from datetime import datetime
import json # Import the json module

def parse_float(value):
    if value:
        try:
            cleaned_value = value.replace(',', '').replace('%', '').strip()
            if cleaned_value == '-' or cleaned_value == '':
                return None
            return float(cleaned_value)
        except ValueError:
            return None
    return None

def parse_int(value):
    if value:
        try:
            cleaned_value = value.replace(',', '').strip()
            if cleaned_value == '-' or cleaned_value == '':
                return None
            return int(float(cleaned_value))
        except ValueError:
            return None
    return None

def parse_date(date_string):
    if date_string and date_string.strip() != '-':
        try:
            for fmt in ('%b %d, %Y', '%d-%m-%Y', '%Y%m%d'):
                try:
                    return datetime.strptime(date_string.strip(), fmt).strftime('%Y-%m-%d')
                except ValueError:
                    pass
            return None
        except TypeError:
            return None
    return None

def scrape_company_data(instr_cd):
    url = f"https://www.dsebd.org/displayCompany.php?name={instr_cd}"
    
    try:
        response = requests.get(url, timeout=10)
        response.raise_for_status() # Raise an HTTPError for bad responses (4xx or 5xx)
    except requests.exceptions.RequestException as e:
        print(f"Error fetching {url}: {e}")
        return None

    soup = BeautifulSoup(response.text, 'html.parser')
    
    company_data = {'Instr_CD': instr_cd}
    
    # --- Company Name ---
    company_name_tag = soup.select_one('h2.BodyHead.topBodyHead i')
    if company_name_tag:
        company_data['CompanyName'] = company_name_tag.get_text(strip=True).replace('Company Name:', '').strip()

    # --- Trading Code and Scrip Code ---
    trading_scrip_table = soup.select_one('table.shares-table')
    if trading_scrip_table:
        # Corrected: td_tags should be from td, not th
        trading_code_th = trading_scrip_table.find('th', string=re.compile(r'Trading Code:'))
        if trading_code_th:
            company_data['TradingCode'] = trading_code_th.get_text(strip=True).replace('Trading Code:', '').strip()
        
        scrip_code_th = trading_scrip_table.find('th', string=re.compile(r'Scrip Code:'))
        if scrip_code_th:
            company_data['ScripCode'] = scrip_code_th.get_text(strip=True).replace('Scrip Code:', '').strip()


    # --- Market Information ---
    market_info_h2 = soup.find('h2', string=lambda text: text and 'Market Information:' in text)
    if market_info_h2:
        market_info_div = market_info_h2.find_next_sibling('div')
        if market_info_div:
            market_info_table = market_info_div.find('table')
            if market_info_table:
                rows = market_info_table.find_all('tr')
                for row in rows:
                    cols = row.find_all(['th', 'td'])
                    if len(cols) == 4:
                        label1 = cols[0].get_text(strip=True)
                        value1 = cols[1].get_text(strip=True)
                        label2 = cols[2].get_text(strip=True)
                        value2 = cols[3].get_text(strip=True)

                        if 'Last Trading Price' in label1: company_data['LastTradingPrice'] = parse_float(value1)
                        if 'Closing Price' in label2: company_data['ClosingPrice'] = parse_float(value2)
                        if 'Last Update' in label1: company_data['LastUpdate'] = value1
                        if 'Day\'s Range' in label2: company_data['DaysRange'] = value2
                        if 'Change*' in label1:
                            company_data['ChangeValue'] = parse_float(value1)
                            if len(cols) > 3:
                                company_data['ChangePercentage'] = parse_float(cols[3].get_text(strip=True)) 
                        if 'Day\'s Value (mn)' in label2: company_data['DaysValue_mn'] = parse_float(value2)
                        if '52 Weeks\' Moving Range' in label2: company_data['FiftyTwoWeeksMovingRange'] = value2
                        if 'Opening Price' in label1: company_data['OpeningPrice'] = parse_float(value1)
                        if 'Day\'s Volume (Nos.)' in label2: company_data['DaysVolume_Nos'] = parse_float(value2)
                        if 'Adjusted Opening Price' in label1: company_data['AdjustedOpeningPrice'] = parse_float(value1)
                        if 'Day\'s Trade (Nos.)' in label2: company_data['DaysTrade_Nos'] = parse_int(value2)
                        if 'Yesterday\'s Closing Price' in label1: company_data['YesterdaysClosingPrice'] = parse_float(value1)
                        if 'Market Capitalization (mn)' in label2: company_data['MarketCapitalization_mn'] = parse_float(value2)
                
                market_date_i = market_info_h2.find('i')
                if market_date_i:
                    company_data['MarketDate'] = parse_date(market_date_i.get_text(strip=True))

    # --- Basic Information ---
    basic_info_h2 = soup.find('h2', string='Basic Information')
    if basic_info_h2:
        basic_info_table = basic_info_h2.find_next_sibling('div').find('table')
        if basic_info_table:
            rows = basic_info_table.find_all('tr')
            for row in rows:
                cols = row.find_all(['th', 'td'])
                if len(cols) == 4:
                    label1 = cols[0].get_text(strip=True)
                    value1 = cols[1].get_text(strip=True)
                    label2 = cols[2].get_text(strip=True)
                    value2 = cols[3].get_text(strip=True)

                    if 'Authorized Capital (mn)' in label1: company_data['AuthorizedCapital_mn'] = parse_float(value1)
                    if 'Debut Trading Date' in label2: company_data['DebutTradingDate'] = parse_date(value2)
                    if 'Paid-up Capital (mn)' in label1: company_data['PaidUpCapital_mn'] = parse_float(value1)
                    if 'Type of Instrument' in label2: company_data['TypeOfInstrument'] = value2
                    if 'Face/par Value' in label1: company_data['FaceValue'] = parse_float(value1)
                    if 'Market Lot' in label2: company_data['MarketLot'] = parse_int(value2)
                    if 'Total No. of Outstanding Securities' in label1: company_data['TotalOutstandingSecurities'] = parse_int(value1)
                    if 'Sector' in label2: company_data['Sector'] = value2

    # --- Last AGM and Year Ended ---
    agm_header = soup.find('h2', string=lambda text: text and 'Last AGM held on:' in text)
    if agm_header:
        agm_i_tag = agm_header.find('i')
        if agm_i_tag:
            company_data['LastAGMDate'] = parse_date(agm_i_tag.get_text(strip=True))
        
        year_ended_divs = agm_header.find_all('div', class_='col-sm-6')
        if len(year_ended_divs) > 1:
            year_ended_text = year_ended_divs[-1].get_text(strip=True)
            match = re.search(r'For the year ended:\s*(.+)', year_ended_text)
            if match:
                company_data['YearEnded'] = parse_date(match.group(1).strip())

    # --- Other Information of the Company ---
    other_info_h2 = soup.find('h2', string='Other Information of the Company')
    if other_info_h2:
        other_info_table = other_info_h2.find_next_sibling('div').find('table')
        if other_info_table:
            rows = other_info_table.find_all('tr')
            for row in rows:
                cols = row.find_all(['td'])
                if len(cols) == 2:
                    label = cols[0].get_text(strip=True)
                    value = cols[1].get_text(strip=True)

                    if 'Listing Year' in label: company_data['ListingYear'] = parse_int(value)
                    if 'Market Category' in label: company_data['MarketCategory'] = value
                    if 'Electronic Share' in label: company_data['ElectronicShare'] = value
                    if 'Remarks' in label: company_data['Remarks'] = value
                elif len(cols) > 0 and 'Share Holding Percentage' in cols[0].get_text(strip=True):
                    # This is a complex nested table, we'll just get the raw text for simplicity
                    company_data['ShareHoldingPercentage_Raw'] = cols[1].get_text(strip=True)


    # --- Corporate Performance at a glance ---
    corp_perf_h2 = soup.find('h2', string='Corporate Performance at a glance')
    if corp_perf_h2:
        corp_perf_table = corp_perf_h2.find_next_sibling('div').find('table')
        if corp_perf_table:
            rows = corp_perf_table.find_all('tr')
            for row in rows:
                cols = row.find_all(['td'])
                if len(cols) >= 2:
                    label = cols[0].get_text(strip=True)
                    value = cols[-1].get_text(strip=True) # Last td is the value

                    if 'Present Operational Status' in label: company_data['PresentOperationalStatus'] = value
                    if 'Short-term loan (mn)' in label: company_data['ShortTermLoan_mn'] = parse_float(value)
                    if 'Long-term loan (mn)' in label: company_data['LongTermLoan_mn'] = parse_float(value)
                    if 'Latest Dividend Status (%)' in label: company_data['LatestDividendStatus_Pct'] = parse_float(value.replace('for 2024', '').strip())
                    # Check if 'Credit Rating' is in the previous row's header for context
                    if 'Short-term' in label and row.find_previous_sibling('tr') and 'Credit Rating' in row.find_previous_sibling('tr').get_text(strip=True): company_data['CreditRating_ShortTerm'] = value
                    if 'Long-term' in label and row.find_previous_sibling('tr') and 'Credit Rating' in row.find_previous_sibling('tr').get_text(strip=True): company_data['CreditRating_LongTerm'] = value
                    if 'OTC/Delisting/Relisting' in label: company_data['OTCDelistingRelisting'] = value

    # --- Address of the Company ---
    address_h2 = soup.find('h2', string='Address of the Company')
    if address_h2:
        address_table = address_h2.find_next_sibling('div').find('table')
        if address_table:
            rows = address_table.find_all('tr')
            for row in rows:
                cols = row.find_all(['td'])
                if len(cols) >= 2:
                    label = cols[0].get_text(strip=True)
                    value = cols[-1].get_text(strip=True)

                    if 'Head Office' in label: company_data['HeadOfficeAddress'] = value
                    if 'Factory' in label: company_data['FactoryAddress'] = value
                    if 'Contact Phone' in label: company_data['ContactPhone'] = value
                    if 'Fax' in label: company_data['Fax'] = value
                    if 'E-mail' in label:
                        # Distinguish company email from CS email by checking previous row's content
                        prev_row_text = row.find_previous_sibling('tr').get_text(strip=True) if row.find_previous_sibling('tr') else ''
                        if 'Company Secretary' not in prev_row_text:
                            company_data['CompanyEmail'] = value
                        else:
                            company_data['CSEmail'] = value
                    if 'Web Address' in label: company_data['WebAddress'] = cols[-1].find('a')['href'] if cols[-1].find('a') else value
                    if 'Company Secretary Name' in label: company_data['CompanySecretaryName'] = value
                    if 'Cell No.' in label: company_data['CSCellNo'] = value
                    if 'Telephone No.' in label: company_data['CSTelephoneNo'] = value
    
    return company_data

if __name__ == "__main__":
    instr_cds_file_path = 'E:\\Project\\Test Projects\\SignalRWithAngular\\instr_cds.txt'
    output_json_path = 'E:\\Project\\Test Projects\\SignalRWithAngular\\company_data.json'

    # Convert Windows path to WSL path for opening the file
    if instr_cds_file_path.startswith('E:\\'):
        wsl_instr_cds_file_path = '/mnt/e/' + instr_cds_file_path[3:].replace('\\', '/')
    else:
        wsl_instr_cds_file_path = instr_cds_file_path # Assume it's already a WSL path or relative

    try:
        with open(wsl_instr_cds_file_path, 'r') as f:
            instr_cds = [line.strip() for line in f if line.strip()]
    except FileNotFoundError:
        print(f"Error: instr_cds.txt not found at {wsl_instr_cds_file_path}")
        exit()

    all_companies_data = []
    # Collect all data first
    for instr_cd in instr_cds:
        print(f"Scraping data for {instr_cd}...")
        data = scrape_company_data(instr_cd)
        if data:
            all_companies_data.append(data)
        else:
            print(f"Failed to scrape data for {instr_cd}")
        print("-" * 30)

    if all_companies_data:
        # Convert Windows output path to WSL path for writing the file
        if output_json_path.startswith('E:\\'):
            wsl_output_json_path = '/mnt/e/' + output_json_path[3:].replace('\\', '/')
        else:
            wsl_output_json_path = output_json_path

        try:
            with open(wsl_output_json_path, 'w', encoding='utf-8') as jsonfile:
                json.dump(all_companies_data, jsonfile, indent=4, ensure_ascii=False)
            print(f"\nScraping complete. Data saved to {wsl_output_json_path}")
        except IOError as e:
            print(f"Error writing to JSON file {wsl_output_json_path}: {e}")
    else:
        print("\nNo data scraped.")