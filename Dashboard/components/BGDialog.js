import React, { Component } from 'react';
import ReactDOM from 'react-dom';
import {Dialog} from '@material-ui/core';

export class BGDialog extends Component {

	constructor(props){
		super(props);
		this.state = {
			open: true,
		};		
	}

	handleClose(){
		this.setState({
			open: false
		});
		ReactDOM.unmountComponentAtNode(document.getElementById('dialogContainer'));
	}

	render(){
		const { children, onClose } = this.props;
		return(
			<Dialog open={this.state.open} onClose={onClose} fullWidth={true} maxWidth = {'md'}>
				{children}
			</Dialog>
		)
	}


}