import React from "react";
import * as Server from "./server";
import { Link } from "react-router-dom";

class NotificationComponent extends React.Component {
  state = {
    notification: null
  };

  notificationDetailsByType(details) {
    switch (details.type) {
      case "message":
        return {
          link: "/messages/" + details.fromUserId,
          icon: "fa fa-commenting fg-info",
          text: "Go to message"
        };
      case "email":
        return {
          link: "/email",
          icon: "fa fa-envelope-square fg-info",
          text: "Go to email"
        };
      case "admin":
        return {
          link: "/settings",
          icon: "fa fa-gear fg-info",
          text: "Go to settings"
        };
    }
  }

  deleteNotification = id => {
    Server.notifications_delete(id);
  };

  componentDidMount() {
    const $ = window.$;

    this.connection = $.hubConnection();
    const notificationHubProxy = this.connection.createHubProxy(
      "notificationHub"
    );

    notificationHubProxy.on("addNotification", message => {
      if (message === "") {
        this.setState({ notification: false });
      } else {
        this.setState({ notification: JSON.parse(message) });
      }
    });
    notificationHubProxy.on("SendNotificationsToUser", message => {
      this.setState({ notification: JSON.parse(message) });
    });

    notificationHubProxy.connection.url = "/signalr";

    this.connection
      .start()
      .done(function() {
        console.log("Connected!");
      })
      .fail(function() {
        console.log("Could not Connect!");
      });
  }

  componentWillUnmount() {
    this.connection.stop();
  }

  render() {
    return (
      <li className="dropdown navbar-notification">
        <a href="#" className="dropdown-toggle" data-toggle="dropdown">
          <i className="fa fa-bell-o" />
          <span className="rounded count label label-danger">
            {this.state.notification && this.state.notification.length}
          </span>
        </a>
        <div className="dropdown-menu animated fadeInDown">
          <div className="dropdown-header">
            <span className="title">
              Notifications{" "}
              <strong>
                ({this.state.notification ? this.state.notification.length : 0})
              </strong>
            </span>
            <span className="option text-right">
              <Link to="/settings">
                <i className="fa fa-cog" /> Setting
              </Link>
            </span>
          </div>
          <div
            className="dropdown-body niceScroll"
            tabIndex="6"
            style={{
              overflow:
                this.state.notification && this.state.notification.length > 4
                  ? "auto"
                  : "visible"
            }}
          >
            {this.state.notification && (
              <div className="media-list small">
                {this.state.notification
                  .sort((a, b) => a.Id - b.Id)
                  .map(notification => {
                    const details = this.notificationDetailsByType(
                      notification.NotificationJson
                    );
                    return (
                      <Link
                        to={details.link}
                        className="media"
                        key={notification.Id}
                      >
                        <div className="media-object pull-left">
                          <i className={details.icon} />
                        </div>
                        <div className="media-body">
                          <span className="media-text">
                            {notification.NotificationJson.fromName
                              ? "Notification from" +
                                notification.NotificationJson.fromName
                              : "You have a notification."}
                            <button
                              type="button"
                              className="btn btn-link pull-right"
                              style={{
                                color: "red"
                              }}
                              onClick={() =>
                                this.deleteNotification(notification.Id)
                              }
                            >
                              &times;
                            </button>
                          </span>
                          <span className="media-meta">{details.text}</span>
                        </div>
                      </Link>
                    );
                  })}
              </div>
            )}
          </div>
          <div className="dropdown-footer">
            {this.state.notification ? (
              <Link to="/notifications">See all </Link>
            ) : (
              "No new notifications"
            )}
          </div>
        </div>
      </li>
    );
  }
}

export default NotificationComponent;
