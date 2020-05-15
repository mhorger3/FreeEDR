import React, { Component } from 'react';
import ReactDOM from 'react-dom';
import axios from 'axios'
import {BgDialog} from '../components/BgDialog.js';
import {Radio, Button, RadioGroup, FormHelperText, FormControlLabel, FormControl, FormLabel, TextField} from '@material-ui/core';

export class BgReportDialog extends BgDialog
{
    constructor(props){
        super(props);
        this.state = {
            selectedStatus: 0,
            format: "",
            email: "",
            returnSet: "",
        }
    };

    static showModal(){
        ReactDOM.render(<BgReportDialog/>, document.getElementById('dialogContainer'));
    };

    checkData(){
        this.handleClose();
    };

    handleEventChange(event){
        this.setState({
            selectedStatus : event.target.value
        });
    };

    handleFormatChange(event){
        this.setState({
            format: event.target.value
        });
    }

    handleEmailChange(event){
        this.setState({
            email: event.taget.value
        });
    }

    processData(selectedStatus, format){
        var emailField = document.getElementById('emailField').value;
        axios.get(`http://localhost:56705/Reporting.svc/GetReportFormat/?name=` + selectedStatus + "&f=" + format)
		  .then(res => {
			const data = res.data.GetReportFormatResult;
            console.log(data);
            if(data != null){
                this.setState({ returnSet: data });
                if(emailField == ""){  
                    this.handleClose();          
                } else {
                    console.log(emailField);
                    axios.get(`http://localhost:56705/Reporting.svc/ExportReport/?path=` + data + "&recipient=" + emailField)
                    .then(res => {
                      const data = res.data.ExportReport;
                      window.alert("Report Exported");
                      this.handleClose();
                    })
                }
            } else {
                this.handleClose();
            }
        });
    }

    render(){
        return(
            <BgDialog onClose={() => {this.checkData()}}>
                <div style={{padding: '1em'}}>
                Generate Report Dialog
                <br></br>
                <br></br>
                <FormControl component="fieldset">
                    Event ID:
                    <RadioGroup row aria-label="Report Option" name="Reports" value={this.state.selectedStatus} onChange={(event) => {this.handleEventChange(event)}}>
                        <FormControlLabel value="1" control={<Radio />} label="1"/>
                        <FormControlLabel value="2" control={<Radio />} label="2"/>
                        <FormControlLabel value="3" control={<Radio />} label="3"/>
                        <FormControlLabel value="4" control={<Radio />} label="4"/>
                        <FormControlLabel value="5" control={<Radio />} label="5"/>
                        <FormControlLabel value="6" control={<Radio />} label="6"/>
                        <FormControlLabel value="7" control={<Radio />} label="7"/>
                        <FormControlLabel value="8" control={<Radio />} label="8"/>
                        <FormControlLabel value="9" control={<Radio />} label="9"/>
                        <FormControlLabel value="10" control={<Radio />} label="10"/>
                        <FormControlLabel value="11" control={<Radio />} label="11"/>
                        <FormControlLabel value="12" control={<Radio />} label="12"/>
                        <FormControlLabel value="13" control={<Radio />} label="13"/>
                        <FormControlLabel value="14" control={<Radio />} label="14"/>
                        <FormControlLabel value="15" control={<Radio />} label="15"/>
                        <FormControlLabel value="17" control={<Radio />} label="17"/>
                        <FormControlLabel value="18" control={<Radio />} label="18"/>
                        <FormControlLabel value="19" control={<Radio />} label="19"/>
                        <FormControlLabel value="20" control={<Radio />} label="20"/>
                        <FormControlLabel value="21" control={<Radio />} label="21"/>
                        <FormControlLabel value="22" control={<Radio />} label="22"/>
                    </RadioGroup>
                    <br></br>
                    Format Option:
                    <RadioGroup row aria-label="Format Option" name="Format" value={this.state.format} onChange={(event) => {this.handleFormatChange(event)}}>
                        <FormControlLabel value="0" control={<Radio />} label="PDF"/>
                        <FormControlLabel value="1" control={<Radio />} label="CSV"/>
                        <FormControlLabel value="2" control={<Radio />} label="TXT"/>
                    </RadioGroup>
                    <br></br>
                    Export via Email:
                    <TextField id='emailField' label=""/>
                    </FormControl>
                <br></br>
                <span style={{display: 'inline', float: 'right'}}>
                    <Button id="generateReport" onClick={() => {this.processData(this.state.selectedStatus, this.state.format)}}>Generate </Button>&nbsp;&nbsp;&nbsp;&nbsp;
                    <Button id="cancelReport" onClick={() => {this.checkData()}}>Cancel </Button>
                </span>
                </div>
            </BgDialog>
        );
    }
}

export class BgTBDDialog extends BgDialog
{
    constructor(props){
        super(props);
        this.state = {
            selectedStatus: 0,
            format: "",
            email: "",
            returnSet: "",
        }
    };

    static showModal(){
        ReactDOM.render(<BgTBDDialog/>, document.getElementById('dialogContainer'));
    };

    checkData(){
        this.handleClose();
    };

    render(){
        return(
            <BgDialog onClose={() => {this.checkData()}}>
                <div style={{padding: '1em'}}>
                Coming Soon...
                <br></br>
                <br></br>
                <span style={{display: 'inline', float: 'right'}}>
                    <Button id="cancelReport" onClick={() => {this.checkData()}}>Cancel </Button>
                </span>
                </div>
            </BgDialog>
        );
    }
}